using API.Data;
using API.DTOs;
using API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DanhGiaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DanhGiaController(ApplicationDbContext context)
        {
            _context = context;
        }
        // Tạo đánh giá mới (chỉ nếu có hoá đơn hoàn tất)
        
        [HttpPost]
        public async Task<IActionResult> TaoDanhGia(DanhGia model)
        {
            try
            {
                var existing = await _context.DanhGias
                .Where(dl => dl.DichVu == model.DichVu && dl.UserId == model.UserId)
                .ToListAsync();

                if (existing.Count > 0)
                {
                    return BadRequest(new { message = "Bạn đã đánh giá dịch vụ này rồi." });
                }

                _context.DanhGias.Add(model);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Đánh giá đã được gửi chờ duyệt." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi nội bộ", error = ex.Message });
            }
        }

        // Xóa đánh giá
        [HttpDelete("{id}")]
        public async Task<IActionResult> Xoa(int id)
        {
            var dg = await _context.DanhGias.FindAsync(id);
            if (dg == null) return NotFound();

            _context.DanhGias.Remove(dg);
            await _context.SaveChangesAsync();
            return Ok("Đã xoá đánh giá.");
        }


        [HttpGet("dichvu/{maDichVu}")]
        public async Task<IActionResult> LayDanhGiaTheoDichVu(int maDichVu)
        {
            var danhGias = await _context.DanhGias
                .Where(d => d.MaDichVu == maDichVu && d.IsActive) // 👈 Bắt buộc thêm IsActive!
                .Include(d => d.User)
                .OrderByDescending(d => d.NgayTao)
                .ToListAsync();

            return Ok(danhGias);
        }
        [HttpGet("dichvu/{maDichVu}/{userID}")]
        public async Task<IActionResult> LayDanhGiaTheoDichVu(int maDichVu, string userID)
        {
            var exists = await _context.DanhGias
                .AnyAsync(d => d.MaDichVu == maDichVu && d.IsActive && d.UserId == userID);

            return Ok(new { hasReview = exists });
        }


        // Admin: lấy tất cả review (bao gồm cả ẩn/hiện)
        [HttpGet("admin/all")]
        public async Task<IActionResult> LayTatCa()
        {
            var danhGias = await _context.DanhGias
                .Include(d => d.DichVu)
                .Include(d => d.User)
                .OrderByDescending(d => d.NgayTao)
                .ToListAsync();

            return Ok(danhGias);
        }

        [HttpGet("admin/highlights")]
        public async Task<IActionResult> LayDanhGiaNoiBat()
        {
            var danhGias = await _context.DanhGias
               .AsNoTracking()
               .Where(d => d.IsActive && d.SoSao == 5)
               .Select(x => new Review
               {
                   Content = x.NoiDung,
                   Rate = x.SoSao,
                   CreatedDate = x.NgayTao,
                   //Name = x.User.Name ?? "Ẩn danh"
                   Name = x.AnDanh? "Ẩn danh": x.User.Name,
               }).OrderByDescending(x => x.CreatedDate).Take(9)
               .ToListAsync();

            return Ok(danhGias);
        }


        [HttpGet("approved")]
        public async Task<IActionResult> GetReviews()
        {
            var danhGias = await _context.DanhGias
                .AsNoTracking()
                .Where(d => d.IsActive && d.SoSao == 5 )
                .Select( x => new Review
                {
                    Content = x.NoiDung,
                    Rate = x.SoSao,
                    CreatedDate = x.NgayTao,
                    Name = x.User.Name ?? "Ẩn danh"
                } ).OrderByDescending(x => x.CreatedDate).Take(10)
                .ToListAsync();

            return Ok(danhGias);
        }


       
        /// Người dùng chỉnh sửa đánh giá của mình
      
        [HttpPut("update/{id}")]
        public async Task<IActionResult> SuaDanhGia(int id, [FromBody] DanhGia model)
        {
            var dg = await _context.DanhGias.FindAsync(id);
            if (dg == null) return NotFound(new { message = "Không tìm thấy đánh giá." });

            // ✅ Chỉ cho phép chủ nhân sửa
            if (dg.UserId != model.UserId)
                return Forbid("Bạn không có quyền sửa đánh giá này.");

            // ✅ Cập nhật nội dung & số sao
            dg.NoiDung = model.NoiDung;
            dg.SoSao = model.SoSao;
            dg.NgayTao = DateTime.Now;
            dg.AnDanh = model.AnDanh;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Đánh giá đã được cập nhật." });
        }

        

        // LẤY TRUNG BÌNH SỐ SAO CỦA DỊCH VỤ
        [HttpGet("trungbinh/{maDichVu}")]
        public async Task<IActionResult> TinhTrungBinh(int maDichVu)
        {
            var trungBinh = await _context.DanhGias
                .Where(d => d.MaDichVu == maDichVu )
                .AverageAsync(d => (double?)d.SoSao) ?? 0;

            return Ok(trungBinh);
        }

        [HttpPut("toggle/{id}")]
        public async Task<IActionResult> ToggleTrangThaiDanhGia(int id)
        {
            var dg = await _context.DanhGias.FindAsync(id);
            if (dg == null) return NotFound();

            dg.IsActive = !dg.IsActive; // Đảo trạng thái
            await _context.SaveChangesAsync();

            var trangThai = dg.IsActive ? "hiện" : "ẩn";
            return Ok($"Đánh giá đã được {trangThai}.");
        }


        [HttpGet("trungbinh-trongso")]
        public async Task<ActionResult<double>> GetTrungBinhTrongSo()
        {
            // Lấy dữ liệu đã duyệt
            var danhGias = await _context.DanhGias
                .Where(d => d.IsActive)
                .ToListAsync();

            // Nếu không có đánh giá đã duyệt → fallback sang tất cả đánh giá active
            if (!danhGias.Any())
            {
                danhGias = await _context.DanhGias
                    .Where(d => d.IsActive)
                    .ToListAsync();
            }

            if (!danhGias.Any())
                return Ok(0);

            // Nhóm theo mã dịch vụ
            var groupByService = danhGias
                .GroupBy(d => d.MaDichVu)
                .Select(g => new
                {
                    MaDichVu = g.Key,
                    SoLuot = g.Count(),
                    DiemTrungBinh = g.Average(x => x.SoSao)
                })
                .ToList();

            // Tính trung bình có trọng số
            double tongDiemTrongSo = groupByService.Sum(s => s.DiemTrungBinh * s.SoLuot);
            double tongSoLuot = groupByService.Sum(s => s.SoLuot);

            double trungBinhTrongSo = tongSoLuot > 0
                ? Math.Round(tongDiemTrongSo / tongSoLuot, 2)
                : 0;

            return Ok(trungBinhTrongSo);
        }

    }

}
