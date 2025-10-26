using API.ChatHub;
using API.Data;
using API.DTOs.DatLich;
using API.Extensions;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DatLichController : ControllerBase
    {
        private readonly IHubContext<BookingHub> _hubContext;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        public DatLichController(ApplicationDbContext context, IHubContext<BookingHub> hubContext, UserManager<User> userManager)
        {
            _context = context;
            _hubContext = hubContext;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DatLich>>> GetDatLich()
        {
            var result = await _context.DatLiches
                    .Include(dl => dl.ChiTietDatLichs)
                    .ThenInclude(ct => ct.DichVu)
                    .ToListAsync();

            return Ok(result);
        }
        [HttpGet("by-date")]
        public async Task<ActionResult<IEnumerable<DatLich>>> GetDatLichByDate(DateTime date)
        {
            try
            {
                var startOfDay = date.Date;
                var endOfDay = startOfDay.AddDays(1).AddTicks(-1); // End of the specified date

                var result = await _context.DatLiches
                    .Include(dl => dl.ChiTietDatLichs)
                    .ThenInclude(ct => ct.DichVu)
                    .Where(dl => dl.ThoiGian >= startOfDay && dl.ThoiGian <= endOfDay)
                    .ToListAsync();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Invalid date format or server error", details = ex.Message });
            }
        }
        [HttpGet("by-user")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "User")]
        public async Task<IActionResult> GetHoaDonByUser()
        {
            var userId = User.GetUserId();
            var user = await _userManager.FindByIdAsync(userId);
            var username = user?.UserName;
            var datLichList = await _context.DatLiches
                .Where(h => h.SoDienThoai == username)
                .Include(h => h.ChiTietDatLichs)
                .ThenInclude(ct => ct.DichVu)
                .OrderByDescending(h => h.ThoiGian)
                .ToListAsync();

            return Ok(datLichList);
        }

        [HttpPost]
        public async Task<IActionResult> DatLich([FromBody] DatLichDTO request)
            {
            int thoiLuong = 30;
            List<DichVu> danhSachDichVu = new();

            if (request.DichVus != null && request.DichVus.Any())
            {
                var dichVuIds = request.DichVus.Select(d => d.DichVuID).ToList();

                danhSachDichVu = _context.DichVus
                    .Where(d => dichVuIds.Contains(d.DichVuID))
                    .ToList();

                if (danhSachDichVu.Count != dichVuIds.Count)
                    return BadRequest("Một hoặc nhiều dịch vụ không tồn tại");

                thoiLuong = danhSachDichVu.Sum(d => d.ThoiGian);
            }

            int soKhung = (int)Math.Ceiling(thoiLuong / 30.0);
            DateTime startTime = request.ThoiGian;

            for (int i = 0; i < soKhung; i++)
            {
                var khung = startTime.AddMinutes(i * 30);

                int count = _context.DatLiches
                    .Count(x =>
                        x.ThoiGian <= khung &&
                        x.ThoiGian.AddMinutes(x.ThoiLuong) > khung
                    );

                if (count >= 5)
                    return BadRequest($"Khung giờ {khung:HH:mm} đã đầy. Vui lòng chọn khung giờ khác.");
            }

            // Tạo bản ghi đặt lịch
            var datLich = new DatLich
            {
                SoDienThoai = request.SoDienThoai,
                ThoiGian = request.ThoiGian,
                ThoiLuong = thoiLuong,
                TrangThai = request.DatTruoc? "Chưa đến": "Đã đến",
                DaThanhToan = false,
                GhiChu = request.GhiChu,
                DatTruoc = request.DatTruoc,
            };

            _context.DatLiches.Add(datLich);
            _context.SaveChanges();

            // Lưu ChiTietDatLich với số lượng
            if (request.DichVus != null && request.DichVus.Any())
            {
                foreach (var dvReq in request.DichVus)
                {
                    _context.ChiTietDatLiches.Add(new ChiTietDatLich
                    {
                        DatLichID = datLich.DatLichID,
                        DichVuID = dvReq.DichVuID,
                        soLuongDV = dvReq.SoLuong
                    });
                }

                _context.SaveChanges();
            }
            var datLichDayDu = await _context.DatLiches
            .Include(dl => dl.ChiTietDatLichs)
            .ThenInclude(ct => ct.DichVu)
            .FirstOrDefaultAsync(dl => dl.DatLichID == datLich.DatLichID);

                await _hubContext.Clients.All.SendAsync("ReceiveBookingNotification", datLichDayDu);

            return Ok("🎉 Đặt lịch thành công!");
        }

        [HttpPut("{id}")]
        public IActionResult CapNhatDatLich(int id, [FromBody] DatLichDTO request)
        {
            var datLich = _context.DatLiches.FirstOrDefault(d => d.DatLichID == id);
            if (datLich == null)
                return NotFound("Không tìm thấy lịch đặt");

            int thoiLuong = 30; // mặc định nếu không có dịch vụ
            List<DichVu> danhSachDichVu = new();

            if (request.DichVus != null && request.DichVus.Any())
            {
                var dichVuIds = request.DichVus.Select(d => d.DichVuID).ToList();

                danhSachDichVu = _context.DichVus
                    .Where(d => dichVuIds.Contains(d.DichVuID))
                    .ToList();

                if (danhSachDichVu.Count != dichVuIds.Count)
                    return BadRequest("Một hoặc nhiều dịch vụ không tồn tại");

                thoiLuong = danhSachDichVu.Sum(d => d.ThoiGian);
            }

            int soKhung = (int)Math.Ceiling(thoiLuong / 30.0);
            DateTime startTime = request.ThoiGian;

            // Kiểm tra trùng lịch (bỏ qua chính lịch đang sửa)
            for (int i = 0; i < soKhung; i++)
            {
                var khung = startTime.AddMinutes(i * 30);

                int count = _context.DatLiches
                    .Count(x =>
                        x.DatLichID != id && // bỏ qua lịch hiện tại
                        x.ThoiGian <= khung &&
                        x.ThoiGian.AddMinutes(x.ThoiLuong) > khung
                    );

                if (count >= 5)
                    return BadRequest($"Khung giờ {khung:HH:mm} đã đầy. Vui lòng chọn khung giờ khác.");
            }

            // Cập nhật thông tin chung
            datLich.SoDienThoai = request.SoDienThoai;
            datLich.ThoiGian = request.ThoiGian;
            datLich.ThoiLuong = thoiLuong;
            datLich.GhiChu = request.GhiChu;
            datLich.DatTruoc = request.DatTruoc;

            // Xóa các chi tiết dịch vụ cũ
            var chiTietCu = _context.ChiTietDatLiches
                .Where(c => c.DatLichID == datLich.DatLichID)
                .ToList();

            _context.ChiTietDatLiches.RemoveRange(chiTietCu);

            // Thêm mới chi tiết dịch vụ kèm số lượng
            if (request.DichVus != null && request.DichVus.Any())
            {
                foreach (var dvReq in request.DichVus)
                {
                    _context.ChiTietDatLiches.Add(new ChiTietDatLich
                    {
                        DatLichID = datLich.DatLichID,
                        DichVuID = dvReq.DichVuID,
                        soLuongDV = dvReq.SoLuong
                    });
                }
            }

            _context.SaveChanges();

            return Ok("✅ Cập nhật đặt lịch thành công!");
        }

        [HttpPut("doitrangthai/{id}")]
        public IActionResult DoiTrangThai(int id)
        {
            var datLich = _context.DatLiches.FirstOrDefault(d => d.DatLichID == id);
            if (datLich == null)
                return NotFound("Không tìm thấy lịch hẹn.");

            if (datLich.TrangThai == "Chưa đến")
            {
                datLich.TrangThai = "Đã đến";
            }
            else if (datLich.TrangThai == "Đã đến")
            {
                datLich.TrangThai = "Chưa đến";
            }
            else
            {
                return BadRequest("Trạng thái không hợp lệ để chuyển đổi.");
            }

            _context.SaveChanges();

            return Ok($"✅ Đã đổi trạng thái thành '{datLich.TrangThai}'.");
        }
        [HttpPut("capnhat-thanhtoan/{id}")]
        public async Task<IActionResult> CapNhatThanhToan(int id)
        {
            var datLich = await _context.DatLiches.FindAsync(id);
            if (datLich == null)
                return NotFound();

            datLich.DaThanhToan = true;
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("slots")]
        public async Task<IActionResult> GetSlotInfo(DateTime ngay)
        {
            var startHour = 8;
            var endHour = 18;
            var result = new List<object>();

            var khungGioList = new List<DateTime>();
            for (int h = startHour; h <= endHour; h++)
            {
                khungGioList.Add(new DateTime(ngay.Year, ngay.Month, ngay.Day, h, 0, 0));
                khungGioList.Add(new DateTime(ngay.Year, ngay.Month, ngay.Day, h, 30, 0));
            }

            var lich = await _context.DatLiches
                .Where(x => x.ThoiGian.Date == ngay.Date)
                .ToListAsync();

            //var now = DateTime.Now;
            var now = DateTime.UtcNow.AddHours(7);

            foreach (var khung in khungGioList)
            {
                if (ngay.Date > now.Date || khung >= now)
                {
                    var count = lich.Count(x =>
                        x.ThoiGian <= khung &&
                        x.ThoiGian.AddMinutes(x.ThoiLuong) > khung
                    );

                    result.Add(new
                    {
                        khungGio = khung.ToString("HH:mm"),
                        conLai = Math.Max(0, 5 - count)
                    });
                }
            }

            return Ok(result);
        }
        [HttpDelete("{datLichId}")]
        public async Task<IActionResult> XoaLich(int datLichId)
        {
            var datLich = await _context.DatLiches
                .Include(dl => dl.ChiTietDatLichs)
                .FirstOrDefaultAsync(dl => dl.DatLichID == datLichId);
            

            if (datLich == null )
                return NotFound("Không tìm thấy lịch cần xóa.");

            if (datLich.DaThanhToan == true)
                return BadRequest("Không xóa được vì đã thanh toán.");

            if (datLich.ChiTietDatLichs.Any())
                _context.ChiTietDatLiches.RemoveRange(datLich.ChiTietDatLichs);

            _context.DatLiches.Remove(datLich);
            await _context.SaveChangesAsync();


            return Ok($"Đã xóa lịch có ID = {datLichId}");
        }


    }
}

