using API.Data;
using API.DTOs.ThongKe;
using API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThongKeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ThongKeController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet("SoLuongSanPham")]
        public IActionResult GetSanPhamSoLuong()
        {
            var now = DateTime.Now;

            // Lấy danh sách hóa đơn trong tháng hiện tại
            var hoaDonIds = _context.HoaDons
                .Where(h => h.NgayTao.Month == now.Month && h.NgayTao.Year == now.Year)
                .Select(h => h.HoaDonID)
                .ToList();

            // Lấy chi tiết hóa đơn có sản phẩm
            var chiTietSanPham = _context.ChiTietHoaDons
                .Where(c => c.SanPhamID != null && hoaDonIds.Contains(c.HoaDonID))
                .ToList();

            // Gom nhóm theo sản phẩm và join với bảng Products để lấy tên
            var grouped = chiTietSanPham
                .GroupBy(c => c.SanPhamID)
                .Join(_context.Products.ToList(),
                      g => g.Key,
                      p => p.ProductId,
                      (g, p) => new
                      {
                          ProductName = p.ProductName,
                          SoLuong = g.Sum(x => x.SoLuongSP)
                      })
                .ToList();

            // Tính tổng số lượng tất cả sản phẩm trong tháng
            var total = grouped.Sum(x => x.SoLuong);

            // Tính phần trăm cho từng sản phẩm, làm tròn 2 chữ số và sắp xếp giảm dần theo phần trăm
            var result = grouped
                .Select(x => new ThongKeSanPhamDTO
                {
                    ProductName = x.ProductName,
                    SoLuong = x.SoLuong,
                    PhanTram = (float)(total > 0 ? Math.Round((x.SoLuong * 100.0) / total, 2) : 0)
                })
                .OrderByDescending(x => x.PhanTram)
                .ToList();

            return Ok(result);
        }

        [HttpGet("SoLuongDichVu")]
        public IActionResult GetDichVuSoLuong()
        {
            var now = DateTime.Now;

            // Lấy danh sách hóa đơn trong tháng hiện tại
            var hoaDonIds = _context.HoaDons
                .Where(h => h.NgayTao.Month == now.Month && h.NgayTao.Year == now.Year)
                .Select(h => h.HoaDonID)
                .ToList();

            // Lấy chi tiết hóa đơn có dịch vụ
            var chiTietDichVu = _context.ChiTietHoaDons
                .Where(c => c.DichVuID != null && hoaDonIds.Contains(c.HoaDonID))
                .ToList();

            // Gom nhóm theo dịch vụ
            var grouped = chiTietDichVu
                .GroupBy(c => c.DichVuID)
                .Join(_context.DichVus.ToList(),
                      g => g.Key,
                      p => p.DichVuID,
                      (g, p) => new
                      {
                          ServiceName = p.TenDichVu,
                          SoLuong = g.Sum(x => x.SoLuongSP)
                      })
                .ToList();

            // Tính tổng số lượng tất cả dịch vụ
            var total = grouped.Sum(x => x.SoLuong);

            // Tính phần trăm
            var result = grouped
                .Select(x => new ThongKeDichVuDTO
                {
                    ServiceName = x.ServiceName,
                    SoLuong = x.SoLuong,
                    PhanTram = (float)(total > 0 ? Math.Round((x.SoLuong * 100.0) / total, 2) : 0)
                })
                .OrderByDescending(x => x.PhanTram)
                .ToList();

            return Ok(result);
        }


        [HttpGet("TongTienThangNay")]
        public IActionResult GetTongTienThangNay()
        {
            var now = DateTime.Now;

            var tongTien = _context.HoaDons
                .Where(h => h.NgayTao.Month == now.Month && h.NgayTao.Year == now.Year)
                .Sum(h => (decimal?)h.TongTien) ?? 0;

            return Ok(new { tongTien });
        }

        [HttpGet("TongTienHomNay")]
        public IActionResult GetTongTienHomNay()
        {
            var today = DateTime.Today;

            var tongTien = _context.HoaDons
                .Where(h => h.NgayTao.Date == today)
                .Sum(h => (decimal?)h.TongTien) ?? 0;

            return Ok(new { tongTien });
        }

        [HttpGet("DoanhThuTheoThang")]
        public IActionResult GetDoanhThuTungNgayTrongThang([FromQuery]int month = 0 , [FromQuery] int year =0 )
        {
            var now = DateTime.Now;
            var m = month == 0 ? now.Month : month;
            var y = year == 0 ?  now.Year : year;

            var result = _context.HoaDons
                .Where(h => h.NgayTao.Month == m && h.NgayTao.Year == y)
                .GroupBy(h => h.NgayTao.Date)
                .Select(g => new
                {
                    period = g.Key.Day,
                    value = g.Sum(x => x.TongTien)
                })
                .OrderBy(x => x.period)
                .ToList();

            return Ok(result);
        }

        [HttpGet("DoanhThuNamHienTai")]
        public IActionResult GetDoanhThuTungThangTrongNam([FromQuery]int year = 0)
        {
            var now = DateTime.Now;
			var y = year == 0 ? now.Year : year;

            var result = _context.HoaDons
                .Where(h => h.NgayTao.Year == y)
                .GroupBy(h => h.NgayTao.Month)
                .Select(g => new
                {
                    period = g.Key,
                    value = g.Sum(x => x.TongTien)
                })
                .OrderBy(x => x.period)
                .ToList();

            return Ok(result);
        }

		 [HttpGet("thongKeHoaDon")]
		 public async Task<ActionResult<IEnumerable<HoaDon>>> GetHoaDon(
			DateTime? startDate = null,
			DateTime? endDate = null,
			 string? sodienthoai = null
			 )
		 {
            var query = _context.HoaDons
			   .Include(vc => vc.voucher)
			   .Include(hd => hd.ChiTietHoaDons)
				   .ThenInclude(ct => ct.DichVu)
			   .Include(hd => hd.ChiTietHoaDons)
				   .ThenInclude(ct => ct.SanPham)
			   .AsQueryable();

            if (startDate.HasValue)
			 {
				 query = query.Where(hd => hd.NgayTao >= startDate.Value);
			 }

			 if (endDate.HasValue)
			 {
				 query = query.Where(hd => hd.NgayTao <= endDate.Value);
			 }
            if (!string.IsNullOrWhiteSpace(sodienthoai))
            {
                query = query.Where(hd => hd.UserID != null && hd.UserID.StartsWith(sodienthoai.Trim()));
            }

            var result = await query.OrderByDescending(hd => hd.NgayTao).ToListAsync();
			 return Ok(result);
		 }
        [HttpGet("thongKeDatLich")]
        public async Task<ActionResult<IEnumerable<DatLich>>> GetDatLich(
    DateTime? startDate = null,
    DateTime? endDate = null, string? sodienthoai = null)
        {
            var query = _context.DatLiches
                .Include(dl => dl.ChiTietDatLichs)
                    .ThenInclude(ct => ct.DichVu)
                .AsQueryable();

            if (startDate.HasValue)
            {
                query = query.Where(dl => dl.ThoiGian >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(dl => dl.ThoiGian <= endDate.Value);
            }
            if (!string.IsNullOrWhiteSpace(sodienthoai))
            {
                query = query.Where(dl => dl.SoDienThoai != null && dl.SoDienThoai.StartsWith(sodienthoai.Trim()));
            }

            var result = await query
                .OrderByDescending(dl => dl.ThoiGian)
                .ToListAsync();

            return Ok(result);
        }

        [HttpGet("lichHenToday")]
        public async Task<IActionResult> GetSoLichHomNay()
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            var count = await _context.DatLiches
                .Where(dl => dl.ThoiGian >= today
                          && dl.ThoiGian < tomorrow
                          && dl.DatTruoc == true)
                .CountAsync();

            return Ok(new { SoLichHomNay = count });
        }
        [HttpGet("dvHoanThanhToday")]
        public async Task<IActionResult> GetThongKeDichVuHomNay()
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);
            var query = _context.ChiTietHoaDons
                .Where(ct => ct.DichVuID != null
                          && ct.HoaDon.NgayTao >= today
                          && ct.HoaDon.NgayTao < tomorrow);
            var soLuotDichVu = await query.CountAsync();


            return Ok(new{SoLuotDichVu = soLuotDichVu});
        }

		[HttpGet("ThuChi")]
		public IActionResult GetThuChi()
		{
			var now = DateTime.Now;

			// Thống kê thu từ HoaDon theo HinhThucThanhToan
			var thu = _context.HoaDons
				.Where(h => h.NgayTao.Month == now.Month && h.NgayTao.Year == now.Year)
				.GroupBy(h => h.HinhThucThanhToan ?? "Không xác định")
				.Select(g => new
				{
					LoaiThu = g.Key,
					SoTien = g.Sum(h => h.TongTien)
				})
				.ToList();

			// Thống kê chi từ InventoryHistory (nhập hàng)
			var chiNhapHang = _context.InventoryHistories
				.Where(h => h.ActionType == "Import"
						 && h.Timestamp.Month == now.Month
						 && h.Timestamp.Year == now.Year)
				.GroupBy(h => h.ActionType)
				.Select(g => new
				{
					LoaiChi = "Nhập hàng",
					SoTien = g.Sum(h => (h.ImportPrice ?? 0m) * (decimal)(h.QuantityChanged))
				})
				.ToList();

			// Thống kê chi từ Expenses (chi phí ngoài nhập hàng)
			var chiKhac = _context.Expenses
				.Where(e => e.Date.Month == now.Month && e.Date.Year == now.Year)
				.GroupBy(e => e.ExpenseType ?? "Không xác định")
				.Select(g => new
				{
					LoaiChi = g.Key,
					SoTien = g.Sum(e => e.Amount)
				})
				.ToList();

			// Kết hợp tất cả chi
			var chi = chiNhapHang.Concat(chiKhac)
				.Select(x => new ThongKeChiDTO
				{
					LoaiChi = x.LoaiChi,
					SoTien = x.SoTien,
					PhanTram = 0 // Sẽ tính lại dưới đây
				})
				.ToList();

			// Tính tổng thu và chi
			var tongThu = thu.Sum(x => x.SoTien);
			var tongChi = chi.Sum(x => x.SoTien);

			// Tính phần trăm
			var result = new ThongKeThuChiDTO
			{
				Thu = thu.Select(x => new ThongKeThuDTO
				{
					LoaiThu = x.LoaiThu,
					SoTien = x.SoTien,
					PhanTram = tongThu > 0 ? (float)Math.Round((x.SoTien * 100.0M / tongThu), 2) : 0
				}).ToList(),
				Chi = chi.Select(x => new ThongKeChiDTO
				{
					LoaiChi = x.LoaiChi,
					SoTien = x.SoTien,
					PhanTram = tongChi > 0 ? (float)Math.Round((x.SoTien * 100.0M / tongChi), 2) : 0
				}).ToList(),
				TongThu = tongThu,
				TongChi = tongChi
			};
			

			return Ok(result);
		}
		// Mới: Theo ngày cụ thể
		[HttpGet("ThuChiByDay")]
		public IActionResult GetThuChiByDay(DateTime day)
		{
			var start = day.Date;
			var end = start.AddDays(1);

			return GetThuChiFiltered(start, end);
		}

		// Mới: Theo tháng và năm
		[HttpGet("ThuChiByMonth")]
		public IActionResult GetThuChiByMonth(int month, int year)
		{
			var start = new DateTime(year, month, 1);
			var end = start.AddMonths(1);

			return GetThuChiFiltered(start, end);
		}

		// Mới: Theo năm
		[HttpGet("ThuChiByYear")]
		public IActionResult GetThuChiByYear(int year)
		{
			var start = new DateTime(year, 1, 1);
			var end = start.AddYears(1);

			return GetThuChiFiltered(start, end);
		}

		// Hàm helper để tính ThuChi theo khoảng thời gian
		private IActionResult GetThuChiFiltered(DateTime start, DateTime end)
		{
			// Thống kê thu từ HoaDon
			var thu = _context.HoaDons
				.Where(h => h.NgayTao >= start && h.NgayTao < end)
				.GroupBy(h => h.HinhThucThanhToan ?? "Không xác định")
				.Select(g => new
				{
					LoaiThu = g.Key,
					SoTien = g.Sum(h => h.TongTien)
				})
				.ToList();

			// Thống kê chi từ InventoryHistory (nhập hàng)
			var chiNhapHang = _context.InventoryHistories
				.Where(h => h.ActionType == "Import" && h.Timestamp >= start && h.Timestamp < end)
				.GroupBy(h => h.ActionType)
				.Select(g => new
				{
					LoaiChi = "Nhập hàng",
					SoTien = g.Sum(h => (h.ImportPrice ?? 0m) * (decimal)(h.QuantityChanged))
				})
				.ToList();

			// Thống kê chi từ Expenses
			var chiKhac = _context.Expenses
				.Where(e => e.Date >= start && e.Date < end)
				.GroupBy(e => e.ExpenseType ?? "Không xác định")
				.Select(g => new
				{
					LoaiChi = g.Key,
					SoTien = g.Sum(e => e.Amount)
				})
				.ToList();

			// Kết hợp chi
			var chi = chiNhapHang.Concat(chiKhac)
				.Select(x => new ThongKeChiDTO
				{
					LoaiChi = x.LoaiChi,
					SoTien = x.SoTien,
					PhanTram = 0 // Sẽ tính sau
				})
				.ToList();

			// Tính tổng
			var tongThu = thu.Sum(x => x.SoTien);
			var tongChi = chi.Sum(x => x.SoTien);

			// Tính phần trăm
			var result = new ThongKeThuChiDTO
			{
				Thu = thu.Select(x => new ThongKeThuDTO
				{
					LoaiThu = x.LoaiThu,
					SoTien = x.SoTien,
					PhanTram = tongThu > 0 ? (float)Math.Round((x.SoTien * 100.0M / tongThu), 2) : 0
				}).ToList(),
				Chi = chi.Select(x => new ThongKeChiDTO
				{
					LoaiChi = x.LoaiChi,
					SoTien = x.SoTien,
					PhanTram = tongChi > 0 ? (float)Math.Round((x.SoTien * 100.0M / tongChi), 2) : 0
				}).ToList(),
				TongThu = tongThu,
				TongChi = tongChi
			};

			return Ok(result);
		}

		[HttpPost("Expense")]
		public async Task<IActionResult> CreateExpense([FromBody] ExpenseDTO dto)
		{
			if (dto == null || string.IsNullOrEmpty(dto.ExpenseType))
			{
				return BadRequest("Dữ liệu chi phí không hợp lệ");
			}

			var expense = new Expense
			{
				Amount = dto.Amount,
				ExpenseType = dto.ExpenseType,
				Date = dto.Date,
				Note = dto.Note
			};
			_context.Expenses.Add(expense);
			await _context.SaveChangesAsync();
			return Ok("Thêm chi phí thành công");
		}

		[HttpGet("Expense")]
		public async Task<IActionResult> GetExpenses()
		{
			var expenses = await _context.Expenses
				.Select(e => new
				{
					e.ExpenseId,
					e.Amount,
					e.ExpenseType,
					e.Date,
					e.Note
				})
				.ToListAsync();
			return Ok(expenses.Any() ? expenses : new List<object>());
		}

		[HttpPut("Expense/{id}")]
		public async Task<IActionResult> UpdateExpense(int id, [FromBody] ExpenseDTO dto)
		{
			if (dto == null || string.IsNullOrEmpty(dto.ExpenseType))
			{
				return BadRequest("Dữ liệu chi phí không hợp lệ");
			}

			var expense = await _context.Expenses.FindAsync(id);
			if (expense == null) return NotFound("Chi phí không tồn tại");

			expense.Amount = dto.Amount;
			expense.ExpenseType = dto.ExpenseType;
			expense.Date = dto.Date;
			expense.Note = dto.Note;
			await _context.SaveChangesAsync();
			return Ok("Cập nhật chi phí thành công");
		}

		[HttpDelete("Expense/{id}")]
		public async Task<IActionResult> DeleteExpense(int id)
		{
			var expense = await _context.Expenses.FindAsync(id);
			if (expense == null) return NotFound("Chi phí không tồn tại");

			_context.Expenses.Remove(expense);
			await _context.SaveChangesAsync();
			return Ok("Xóa chi phí thành công");
		}
	}
}
