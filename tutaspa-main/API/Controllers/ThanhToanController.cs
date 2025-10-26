using API.Data;
using API.DTOs.HoaDon;
using API.DTOs.ThongKe;
using API.Models;
using API.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Net.payOS;
using Net.payOS.Types;
using System.Text;
using Twilio.TwiML.Voice;
using DinkToPdf.Contracts;
using API.Extensions;
using DinkToPdf;
using Microsoft.AspNetCore.Identity;
using DocumentFormat.OpenXml.InkML;
using SelectPdf;
using API.DTOs.Product;
using API.IService;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThanhToanController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly PayOS _payOS;
        private readonly IConverter _converter;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<User> _userManager;
        private readonly IInventoryService _inventoryService;

        public ThanhToanController(ApplicationDbContext context, IConfiguration configuration, IConverter converter, IWebHostEnvironment env, UserManager<User> userManager, IInventoryService inventoryService)
        {
            _context = context;
            string clientId = configuration["PayOSConfig:ClientId"] ?? throw new ArgumentNullException("ClientId");
            string apiKey = configuration["PayOSConfig:ApiKey"] ?? throw new ArgumentNullException("ApiKey");
            string checksumKey = configuration["PayOSConfig:ChecksumKey"] ?? throw new ArgumentNullException("ChecksumKey");

            _payOS = new PayOS(clientId, apiKey, checksumKey);
            _converter = converter;
            _env = env;
            _userManager = userManager;
            _inventoryService = inventoryService;
        }
        

        [HttpPost("create-link")]
        public async Task<IActionResult> CreatePaymentLink([FromBody] PaymentDTO request)
        {
            if (request == null || request.Items == null || !request.Items.Any())
            {
                return BadRequest("Request hoặc items không hợp lệ");
            }

            var items = request.Items.Select(i => new ItemData(i.Name, i.Quantity, i.Amount)).ToList();
            int orderId = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            var paymentData = new PaymentData(
                orderId,
                request.TotalAmount,
                request.Description,
                items,
                request.cancelUrl,
                request.returnUrl
            );

            try
            {
                CreatePaymentResult result = await _payOS.createPaymentLink(paymentData);

                return Ok(new
                {
                    qrCode = result.qrCode,
                    checkoutUrl = result.checkoutUrl,
                    orderId = orderId
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi tạo link thanh toán: {ex.Message}");
            }
        }

        [HttpGet("by-user")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "User")]
        public async Task<IActionResult> GetHoaDonByUser()
        {
            var userId = User.GetUserId();
            var user = await _userManager.FindByIdAsync(userId);
            var username = user?.UserName;
            var hoaDonList = await _context.HoaDons
                .Where(h => h.UserID == username)
                .Include(h => h.ChiTietHoaDons)
                .ThenInclude(ct => ct.DichVu)
                .OrderByDescending(h => h.NgayTao)
                .ToListAsync();

            return Ok(hoaDonList);
        }


        [HttpPost("tao-hoadon")]
        public async Task<IActionResult> TaoHoaDon([FromBody] HoaDonDTO request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var tongTien = request.ChiTietHoaDon.Sum(x => x.ThanhTien);
                var voucher = await _context.Vouchers
                .FirstOrDefaultAsync(v => v.MaCode == request.MaGiamGia);
                if (voucher != null)
                {
                    if (voucher.SoLuong > 0)
                    {
                        voucher.SoLuong -= 1;
                        _context.Vouchers.Update(voucher);
                    }
                    tongTien -= (decimal)request.tienGiam;
                }
                

                var hoaDon = new HoaDon
                {
                    NgayTao = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")),
                    HinhThucThanhToan = request.HinhThucThanhToan,
                    TrangThai = request.TrangThai,
                    TienKhachDua = request.TienKhachDua,
                    TienThoiLai = request.TienThoiLai,
                    TongTien = tongTien,
                    GiaTriGiam = request.tienGiam,
                    NhanVienID = request.NhanVienID,
                    UserID = request.UserID,
                    VoucherID = voucher?.VoucherID,
                };

                _context.HoaDons.Add(hoaDon);
                await _context.SaveChangesAsync();

                foreach (var item in request.ChiTietHoaDon)
                {
                    var chiTiet = new ChiTietHoaDon
                    {
                        HoaDonID = hoaDon.HoaDonID,
                        SanPhamID = item.SanPhamID,
                        DichVuID = item.DichVuID,
                        SoLuongSP = item.SoLuongSP,
                        ThanhTien = item.ThanhTien
                    };
                    _context.ChiTietHoaDons.Add(chiTiet);
                    if (item.SanPhamID.HasValue)
                    {
                        var result = await _inventoryService.ExportWithBatchAsync(new ExportProductRequest
                        {
                            ProductId = item.SanPhamID.Value,
                            Quantity = item.SoLuongSP,
                            Note = "Xuất khi tạo hóa đơn #" + hoaDon.HoaDonID
                        });

                        if (!result)
                        {
                            await transaction.RollbackAsync();
                            return BadRequest(new { message = $"Sản phẩm ID {item.SanPhamID} không đủ tồn kho." });
                        }
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { message = "Tạo hóa đơn thành công", hoaDonID = hoaDon.HoaDonID });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { message = "Lỗi khi tạo hóa đơn", error = ex.Message });
            }
        }

       

    [HttpGet("xuat-hoadon/{hoaDonId}")]
        public IActionResult XuatHoaDon1(int hoaDonId)
        {
            var hoaDon = _context.HoaDons
                .Include(vc => vc.voucher)
                .Include(h => h.ChiTietHoaDons)
                    .ThenInclude(ct => ct.SanPham)
                .Include(h => h.ChiTietHoaDons)
                    .ThenInclude(ct => ct.DichVu)
                .FirstOrDefault(h => h.HoaDonID == hoaDonId);

            if (hoaDon == null)
                return NotFound("Không tìm thấy hóa đơn");

            var html = new StringBuilder();

            html.Append(@"
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset='UTF-8'>
            <style>
                * { font-family: Arial, sans-serif; font-size: 12px; }
                h2 { text-align: center; margin-bottom: 5px; font-size: 16px; }
                table { width: 100%; border-collapse: collapse; margin-top: 10px; }
                th, td { padding: 4px; border: 1px solid #000; }
                .right { text-align: right; }
                .center { text-align: center; }
                .bold { font-weight: bold; }
                .line { border-top: 1px dashed #000; margin: 5px 0; }
            </style>
        </head>
        <body>
        ");

            html.Append("<h2>TUTA SPA</h2>");
            html.Append("<div class='center'>--- HÓA ĐƠN ---</div>");
            html.Append($"<p><strong>Mã hóa đơn:</strong> {hoaDon.HoaDonID}<br>");
            html.Append($"<strong>Ngày:</strong> {hoaDon.NgayTao:dd/MM/yyyy HH:mm}<br>");
            html.Append($"<strong>NV:</strong> {hoaDon.NhanVienID ?? "Không rõ"}<br>");
            html.Append($"<strong>Khách:</strong> {hoaDon.UserID ?? "Không rõ"}</p>");
            html.Append("<div class='line'></div>");

            html.Append("<table>");
            html.Append("<thead><tr><th>#</th><th>Tên</th><th>Giá</th><th>Số lượng</th><th>Thành tiền</th></tr></thead>");
            html.Append("<tbody>");

            int stt = 1;
            foreach (var item in hoaDon.ChiTietHoaDons)
            {
                var ten = item.DichVu?.TenDichVu ?? item.SanPham?.ProductName ?? "Không rõ";
                var gia = item.DichVu?.Gia ?? item.SanPham?.CurrentSellingPrice?? 0;
                var soLuong = item.SoLuongSP ;
                var thanhTien = item.ThanhTien;

                html.Append($@"
            <tr>
                <td class='center'>{stt++}</td>
                <td>{ten}</td>
                <td class='right'>{gia:N0}đ</td>
                <td class='center'>{soLuong}</td>
                <td class='right'>{thanhTien:N0}đ</td>
            </tr>");
            }

            html.Append("</tbody></table>");
            html.Append("<div class='line'></div>");

            html.Append("<p class='right'>");
            html.Append($"<strong>Tổng: </strong> {hoaDon.TongTien:N0}đ<br>");
            if (hoaDon.GiaTriGiam != null && hoaDon.GiaTriGiam != 0)
            {
                html.Append($"<strong>Giảm giá: </strong> {hoaDon.GiaTriGiam:N0}đ<br>");
            }
            html.Append($"<strong>Khách đưa: </strong> {hoaDon.TienKhachDua:N0}đ<br>");
            html.Append($"<strong>Thối lại: </strong> {hoaDon.TienThoiLai:N0}đ");
            html.Append("</p>");

            html.Append("<div style='margin-top:15px;text-align:center;'>Cảm ơn quý khách!</div>");
            html.Append("</body></html>");

            // Tạo PDF bằng Select.HtmlToPdf.NetCore
            var converter = new HtmlToPdf();
            converter.Options.PdfPageSize = PdfPageSize.Custom;
            converter.Options.PdfPageCustomSize = new System.Drawing.SizeF(80, 297); // Khổ 80mm x 297mm
            converter.Options.PdfPageOrientation = PdfPageOrientation.Portrait;
            converter.Options.MarginTop = 3;
            converter.Options.MarginBottom = 3;
            converter.Options.MarginLeft = 2;
            converter.Options.MarginRight = 2;
            converter.Options.WebPageWidth = 80;

            var pdf = converter.ConvertHtmlString(html.ToString());
            var pdfBytes = pdf.Save();
            pdf.Close();

            return File(pdfBytes, "application/pdf", $"HoaDon_{hoaDonId}.pdf");
        }

    }
}
