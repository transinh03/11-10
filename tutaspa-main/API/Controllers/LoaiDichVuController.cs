using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Data;
using API.Models;
using ClosedXML.Excel;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoaiDichVuController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LoaiDichVuController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/LoaiDichVus
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LoaiDichVu>>> GetLoaiDichVus()
        {
            return await _context.LoaiDichVus.ToListAsync();
        }

        // GET: api/LoaiDichVus/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LoaiDichVu>> GetLoaiDichVu(int id)
        {
            var loaiDichVu = await _context.LoaiDichVus.FindAsync(id);

            if (loaiDichVu == null)
            {
                return NotFound();
            }

            return loaiDichVu;
        }

        // PUT: api/LoaiDichVus/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLoaiDichVu(int id, LoaiDichVu loaiDichVu)
        {
            if (id != loaiDichVu.LoaiDichVuID)
            {
                return BadRequest();
            }

            _context.Entry(loaiDichVu).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LoaiDichVuExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/LoaiDichVus
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // POST: api/LoaiDichVus
        [HttpPost]
        public async Task<ActionResult<LoaiDichVu>> PostLoaiDichVu(LoaiDichVu loaiDichVu)
        {
            if (string.IsNullOrWhiteSpace(loaiDichVu.TenLoai))
            {
                return BadRequest(new { message = "Tên loại dịch vụ không được để trống." });
            }

            if (loaiDichVu.TenLoai.Any(char.IsDigit))
            {
                return BadRequest(new { message = "Tên loại dịch vụ không được chứa số." });
            }
            var maxId = await _context.LoaiDichVus.MaxAsync(x => (int?)x.LoaiDichVuID) ?? 0;
            loaiDichVu.maLoaiDichVu = $"dv{maxId + 1}";

            var tenLoaiNormalized = loaiDichVu.TenLoai.Trim().ToLower();
            var existed = await _context.LoaiDichVus
                .AnyAsync(x => x.TenLoai.Trim().ToLower() == tenLoaiNormalized);

            if (existed)
            {
                return BadRequest(new { message = "Tên loại dịch vụ đã tồn tại." });
            }

            _context.LoaiDichVus.Add(loaiDichVu);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLoaiDichVu", new { id = loaiDichVu.LoaiDichVuID }, loaiDichVu);
        }


        // DELETE: api/LoaiDichVus/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLoaiDichVu(int id)
        {
            var loaiDichVu = await _context.LoaiDichVus.FindAsync(id);
            if (loaiDichVu == null)
            {
                return NotFound();
            }

            _context.LoaiDichVus.Remove(loaiDichVu);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        private bool LoaiDichVuExists(int id)
        {
            return _context.LoaiDichVus.Any(e => e.LoaiDichVuID == id);
        }
        [HttpPost("import")]
        public async Task<IActionResult> ImportLoaiDichVu(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Chưa chọn file.");

            if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                return BadRequest("File không hợp lệ, chỉ hỗ trợ .xlsx");

            var loaiDVList = new List<LoaiDichVu>();

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var workbook = new XLWorkbook(stream))
                {
                    var worksheet = workbook.Worksheet("LoaiDichVu");
                    if (worksheet == null)
                        return BadRequest("Không tìm thấy sheet tên 'LoaiDichVu'.");

                    foreach (var row in worksheet.RowsUsed().Skip(1)) // Bỏ qua dòng tiêu đề
                    {
                        var tenLoai = row.Cell(1).GetString()?.Trim();
                        if (!string.IsNullOrEmpty(tenLoai))
                        {
                            loaiDVList.Add(new LoaiDichVu { TenLoai = tenLoai });
                        }
                    }
                }
            }

            _context.LoaiDichVus.AddRange(loaiDVList);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, count = loaiDVList.Count });
        }
        [HttpPost("importt")]
        public async Task<IActionResult> ImportData(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Chưa chọn file.");

            if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                return BadRequest("File không hợp lệ, chỉ hỗ trợ .xlsx");

            var loaiDVList = new List<LoaiDichVu>();
            var dichVuList = new List<DichVu>();

            // Biến thống kê
            var skippedLoaiDV = new List<string>();
            var skippedDV = new List<string>();

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var workbook = new XLWorkbook(stream))
                {
                    // ===== Sheet LoaiDichVu =====
                    var sheetLoai = workbook.Worksheets.FirstOrDefault(s => s.Name == "LoaiDichVu");
                    if (sheetLoai != null)
                    {
                        var existedLoaiDVNames = await _context.LoaiDichVus
                            .Select(x => x.TenLoai.ToLower())
                            .ToListAsync();

                        foreach (var row in sheetLoai.RowsUsed().Skip(1))
                        {
                            var tenLoai = row.Cell(1).GetString()?.Trim();
                            var maLoai = row.Cell(2).GetString()?.Trim();

                            if (!string.IsNullOrEmpty(maLoai) && !string.IsNullOrEmpty(tenLoai))
                            {
                                if (existedLoaiDVNames.Contains(tenLoai.ToLower()) ||
                                   loaiDVList.Any(x => x.TenLoai.Equals(tenLoai, StringComparison.OrdinalIgnoreCase)))
                                {
                                    skippedLoaiDV.Add(tenLoai);
                                    continue; // Bỏ qua dòng trùng
                                }

                                loaiDVList.Add(new LoaiDichVu
                                {
                                    maLoaiDichVu = maLoai,
                                    TenLoai = tenLoai
                                });
                            }
                        }
                    }

                    if (loaiDVList.Any())
                    {
                        _context.LoaiDichVus.AddRange(loaiDVList);
                        await _context.SaveChangesAsync();
                    }

                    // ===== Sheet DichVu =====
                    var sheetDV = workbook.Worksheets.FirstOrDefault(s => s.Name == "DichVu");
                    if (sheetDV != null)
                    {
                        var existedDVNames = await _context.DichVus
                            .Select(x => x.TenDichVu.ToLower())
                            .ToListAsync();

                        foreach (var row in sheetDV.RowsUsed().Skip(1))
                        {
                            try
                            {
                                var tenDichVu = row.Cell(1).GetString()?.Trim();
                                var gia = row.Cell(2).GetValue<decimal>();
                                var thoiGian = row.Cell(3).GetValue<int>();
                                var moTa = row.Cell(4).GetString()?.Trim();
                                var hinhAnh = row.Cell(5).GetString()?.Trim();
                                var trangThai = row.Cell(6).GetValue<int>();
                                var ngayTao = row.Cell(7).GetDateTime();
                                var maDichVu = row.Cell(8).GetString()?.Trim();

                                var loaiDV = await _context.LoaiDichVus.FirstOrDefaultAsync(x => x.maLoaiDichVu == maDichVu);
                                if (loaiDV == null) continue;

                                int loaiID = loaiDV.LoaiDichVuID;

                                if (string.IsNullOrEmpty(tenDichVu) || gia < 0 || thoiGian <= 0)
                                    continue;

                                if (existedDVNames.Contains(tenDichVu.ToLower()) ||
                                    dichVuList.Any(x => x.TenDichVu.Equals(tenDichVu, StringComparison.OrdinalIgnoreCase)))
                                {
                                    skippedDV.Add(tenDichVu);
                                    continue;
                                }

                                dichVuList.Add(new DichVu
                                {
                                    maDichVu = maDichVu,
                                    TenDichVu = tenDichVu,
                                    Gia = gia,
                                    ThoiGian = thoiGian,
                                    MoTa = moTa,
                                    HinhAnh = hinhAnh,
                                    TrangThai = trangThai,
                                    NgayTao = ngayTao,
                                    LoaiDichVuID = loaiID,
                                });
                            }
                            catch
                            {
                                skippedDV.Add("Dòng lỗi dữ liệu");
                                continue;
                            }
                        }
                    }
                }
            }

            if (dichVuList.Any())
   {
       _context.DichVus.AddRange(dichVuList);
       await _context.SaveChangesAsync();

       return Ok(new
       {
           success = true,
           loaiDichVuAdded = loaiDVList.Count,
           loaiDichVuSkipped = skippedLoaiDV.Count,
           loaiDichVuSkippedList = skippedLoaiDV,   // Danh sách tên bị bỏ qua
           dichVuAdded = dichVuList.Count,
           dichVuSkipped = skippedDV.Count,
           dichVuSkippedList = skippedDV            // Danh sách tên bị bỏ qua
       });
   }

   return BadRequest(new
   {
       success = false,
       loaiDichVuAdded = loaiDVList.Count,
       loaiDichVuSkipped = skippedLoaiDV.Count,
       loaiDichVuSkippedList = skippedLoaiDV,   // Danh sách tên bị bỏ qua
       dichVuAdded = dichVuList.Count,
       dichVuSkipped = skippedDV.Count,
       dichVuSkippedList = skippedDV            // Danh sách tên bị bỏ qua
   });     
        }

    }
}
