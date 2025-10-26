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
using API.DTOs;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DichVuController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        public DichVuController(ApplicationDbContext context, IWebHostEnvironment env )
        {
            _context = context;
            _env = env;
        }
        [HttpGet("filter")]
        public async Task<ActionResult<object>> GetDichVus(
        [FromQuery] string keyword = "",
        [FromQuery] int? cateId = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12)
        {
            try
            {
                // Validate page parameters
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 12;
                if (pageSize > 50) pageSize = 50; // Giới hạn tối đa

                // Build query
                var query = _context.DichVus
                    .Include(dv => dv.LoaiDichVu)
                    .Where(dv => dv.TrangThai == 1)
                    .AsQueryable();

                // Filter by keyword
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    var searchKeyword = keyword.Trim().ToLower();
                    query = query.Where(dv =>
                        dv.TenDichVu.ToLower().Contains(searchKeyword) ||
                        dv.MoTa.ToLower().Contains(searchKeyword)
                    );
                }

                // Filter by category
                if (cateId.HasValue && cateId.Value > 0)
                {
                    query = query.Where(dv => dv.LoaiDichVuID == cateId.Value);
                }

                // Get total count before pagination
                var totalItems = await query.CountAsync();
                var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

                // Apply pagination and ordering
                var dichVus = await query
                    .OrderByDescending(dv => dv.NgayTao)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(dv => new
                    {
                        dichVuID = dv.DichVuID,
                        tenDichVu = dv.TenDichVu,
                        gia = dv.Gia,
                        thoiGian = dv.ThoiGian,
                        moTa = dv.MoTa,
                        hinhAnh = dv.HinhAnh,
                        ngayTao = dv.NgayTao,
                        trangThai = dv.TrangThai,
                        loaiDichVuID = dv.LoaiDichVuID,
                        tenLoai = dv.LoaiDichVu.TenLoai,
                        // Thêm rating trung bình nếu có bảng đánh giá
                        mucDanhGia = _context.DanhGias
    .Where(dg => dg.MaDichVu == dv.DichVuID && dg.IsActive)
    .Average(dg => (double?)dg.SoSao) ?? 0.0
                    })
.ToListAsync();


                // Return paginated response
                var response = new
                {
                    success = true,
                    data = dichVus,
                    pagination = new
                    {
                        currentPage = page,
                        pageSize = pageSize,
                        totalItems = totalItems,
                        totalPages = totalPages,
                        hasNextPage = page < totalPages,
                        hasPreviousPage = page > 1
                    },
                    filters = new
                    {
                        keyword = keyword,
                        cateId = cateId
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi khi tải danh sách dịch vụ",
                    error = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DichVu>>> GetDichVus()
        {
            var result = await _context.DichVus
        .Where(dv => dv.TrangThai == 1)
        .GroupJoin(
        _context.DanhGias.Where(x => x.IsActive),
        dv => dv.DichVuID,
        dg => dg.MaDichVu,
        (dv, dgs) => new
        {
            dv.DichVuID,
            dv.TenDichVu,
            dv.TrangThai,
            dv.Gia,
            dv.ThoiGian,
            dv.MoTa,
            dv.HinhAnh,
            dv.NgayTao,
            dv.LoaiDichVuID,
            dv.maDichVu,
            MucDanhGia = dgs.Any() ? dgs.Average(x => x.SoSao) : 0
        }
    )
    .ToListAsync();
            return Ok(result);
        }


        [HttpGet("detail/{id}")]
        public async Task<ActionResult<DichVuDTO>> GetDichVusAsync(int id)
        {
            var result = await _context.DichVus
                .Where(dv => dv.DichVuID == id)
                .Select(dv => new DichVuDTO
                {
                    Id = dv.DichVuID,
                    TenDichVu = dv.TenDichVu,
                    Mota = dv.MoTa,
                    Gia = dv.Gia,
                    ThoiGian = dv.ThoiGian,
                    HinhAnh = dv.HinhAnh,
                    LoaiDichVuID = dv.LoaiDichVuID,
                    LoaiDichVu = dv.LoaiDichVu
                })
                .FirstOrDefaultAsync();

            return result != null ? Ok(result) : NotFound();

        }

        [HttpGet("{id}/related")]
        public async Task<ActionResult<List<DichVu>>> GetRelatedDichVus(int id)
        {
            var pivot = await _context.DichVus.AsNoTracking()
                .Where(x => x.DichVuID == id && x.TrangThai == 1)
                .Select(x => new { x.DichVuID, x.LoaiDichVuID, x.Gia, x.ThoiGian })
                .SingleOrDefaultAsync();

            if (pivot == null)
                return NotFound("Dịch vụ không tồn tại hoặc đã ngưng hoạt động.");

            var relatedByCate = await _context.DichVus.AsNoTracking()
                .Where(x => x.LoaiDichVuID == pivot.LoaiDichVuID
                            && x.DichVuID != pivot.DichVuID
                            && x.TrangThai == 1)
                .OrderByDescending(x => x.NgayTao)
                .Take(5)
                .ToListAsync();

            if (relatedByCate.Any())
                return Ok(relatedByCate);

            var minPrice = pivot.Gia * (1 - 0.20m);
            var maxPrice = pivot.Gia * (1 + 0.20m);
            var minTime = pivot.ThoiGian - 15;
            var maxTime = pivot.ThoiGian + 15;

            var relatedByPriceOrTime = await _context.DichVus.AsNoTracking()
                .Where(x => x.TrangThai == 1
                            && x.DichVuID != pivot.DichVuID
                            && (
                                 (x.Gia >= minPrice && x.Gia <= maxPrice)
                                 || (x.ThoiGian >= minTime && x.ThoiGian <= maxTime)
                               )
                      )
                .OrderByDescending(x => x.NgayTao)
                .Take(5)
                .ToListAsync();

            if (relatedByPriceOrTime.Any())
                return Ok(relatedByPriceOrTime);

            return NotFound();
        }

        [HttpGet("{id}/reviews")]
        public async Task<ActionResult<ReviewsDTO>> GetReviews(int id)
        {
            var reviews = await _context.DanhGias.AsNoTracking()
                .Where(r => r.MaDichVu == id && r.IsActive)
                .Select(r => new Review
                {
                    Rate = r.SoSao,
                    Name = r.User.Name ?? "Ẩn danh",
                    Content = r.NoiDung,
                    CreatedDate = r.NgayTao
                })
                .ToListAsync();
            if (reviews == null)
                return NotFound();

            var result = new ReviewsDTO
            {
                DichVuId = id,
                Rating = reviews.Any() ? reviews.Average(r => r.Rate) : 0,
                Reviews = reviews
            };

            return Ok(result);
        }


        // ✅ (Tùy chọn) Trả về tất cả dịch vụ (admin dùng)
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<object>>> GetAllDichVus()
        {
            var dichVus = await _context.DichVus
                .Include(dv => dv.LoaiDichVu) // Join với bảng LoaiDichVu
                .Select(dv => new
                {
                    dv.DichVuID,
                    dv.TenDichVu,
                    dv.Gia,
                    dv.ThoiGian,
                    dv.MoTa,
                    dv.HinhAnh,
                    dv.NgayTao,
                    dv.TrangThai,
                    dv.LoaiDichVuID,
                    dv.LoaiDichVu.TenLoai // Lấy tên loại
                })
                .ToListAsync();

            return Ok(dichVus);
        }

        // GET: api/DichVus/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DichVu>> GetDichVu(int id)
        {
            var dichVu = await _context.DichVus.FindAsync(id);

            if (dichVu == null)
            {
                return NotFound();
            }

            return dichVu;
        }
        // GET: api/DichVu/loai
        [HttpGet("loai")]
        public async Task<ActionResult<IEnumerable<object>>> GetLoaiDichVus()
        {
            var loaiList = await _context.LoaiDichVus
                .Select(ldv => new {
                    ldv.LoaiDichVuID,
                    ldv.TenLoai
                })
                .ToListAsync();

            return Ok(loaiList);
        }
        // PUT: api/DichVus/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDichVu(int id, DichVu dichVu)
        {
            if (id != dichVu.DichVuID)
            {
                return BadRequest();
            }

            _context.Entry(dichVu).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DichVuExists(id))
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

        // POST: api/DichVus
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<DichVu>> PostDichVu(DichVu dichVu)
        {
            if (string.IsNullOrWhiteSpace(dichVu.TenDichVu))
            {
                return BadRequest(new { message = "Tên dịch vụ không được để trống." });
            }

            if (dichVu.TenDichVu.Any(char.IsDigit))
            {
                return BadRequest(new { message = "Tên dịch vụ không được chứa số." });
            }

            var tenDichVuNormalized = dichVu.TenDichVu.Trim().ToLower();

            var existedDV = await _context.DichVus
                .FirstOrDefaultAsync(x => x.TenDichVu.Trim().ToLower() == tenDichVuNormalized);

            if (existedDV != null)
            {
                if (existedDV.LoaiDichVuID != dichVu.LoaiDichVuID)
                {
                    return BadRequest(new { message = $"Dịch vụ '{dichVu.TenDichVu}' đã tồn tại trong loại khác, vui lòng chọn đúng loại dịch vụ." });
                }
                else
                {
                    return BadRequest(new { message = "Tên dịch vụ đã tồn tại trong loại này." });
                }
            }

            if (dichVu.Gia <= 0)
            {
                return BadRequest(new { message = "Giá dịch vụ phải lớn hơn 0." });
            }

            if (dichVu.ThoiGian <= 0)
            {
                return BadRequest(new { message = "Thời gian phải lớn hơn 0." });
            }

            if (string.IsNullOrWhiteSpace(dichVu.HinhAnh))
            {
                return BadRequest(new { message = "Hình ảnh không được để trống." });
            }
            var loaiDV = await _context.LoaiDichVus.FirstOrDefaultAsync(x => x.LoaiDichVuID == dichVu.LoaiDichVuID);
            if (loaiDV == null)
            {
                return BadRequest(new { message = "Loại dịch vụ không tồn tại." });
            }

            // ✅ Gán maDichVu = maLoaiDichVu
            dichVu.maDichVu = loaiDV.maLoaiDichVu;

            dichVu.NgayTao = DateTime.Now;
            _context.DichVus.Add(dichVu);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDichVu", new { id = dichVu.DichVuID }, dichVu);
        }
        [HttpGet("name")]
        public async Task<ActionResult<IEnumerable<DichVu>>> SearchByName(string ten)
        {
            if (string.IsNullOrWhiteSpace(ten))
                return BadRequest("Vui lòng nhập tên dịch vụ cần tìm.");

            var keyword = ten.ToLower();

            var result = await _context.DichVus
                .Where(d => d.TenDichVu.ToLower().Contains(keyword))
                .ToListAsync();

            if (result == null || result.Count == 0)
                return NotFound("Không tìm thấy dịch vụ phù hợp.");

            return Ok(result);
        }
        // GET: api/DichVu/filter-by-price?min=50000&max=100000
        [HttpGet("filter-by-price")]
        public async Task<ActionResult<IEnumerable<DichVu>>> FilterByPrice(decimal min, decimal max)
        {
            if (min < 0 || max < 0 || min > max)
            {
                return BadRequest(new { Message = "Khoảng giá không hợp lệ." });
            }
            var result = await _context.DichVus
                .Where(d => d.Gia >= min && d.Gia <= max)
                .ToListAsync();

            if (result == null || result.Count == 0)
                return NotFound("Không tìm thấy dịch vụ trong khoảng giá đã nhập.");

            return Ok(result);
        }
        // PUT: api/DichVu/toggle-status/5
        [HttpPut("toggle-status/{id}")]
        public async Task<IActionResult> ToggleTrangThai(int id)
        {
            var dichVu = await _context.DichVus.FindAsync(id);

            if (dichVu == null)
            {
                return NotFound(new { Message = "Không tìm thấy dịch vụ." });
            }

            // Giả sử: 1 = Hoạt động, 2 = Ngừng hoạt động
            dichVu.TrangThai = (dichVu.TrangThai == 1) ? 2 : 1;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    Message = "Cập nhật trạng thái thành công.",
                    TrangThaiMoi = dichVu.TrangThai == 1 ? "Hoạt động" : "Ngừng hoạt động"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Lỗi khi cập nhật trạng thái.", Error = ex.Message });
            }
        }
        [HttpPost("image")]
        public async Task<IActionResult> UploadImage(IFormFile file, [FromForm] string tenDichVu)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "Không có file ảnh." });

            if (string.IsNullOrWhiteSpace(tenDichVu))
                return BadRequest(new { message = "Tên dịch vụ không được rỗng." });

            var uploadsFolder = Path.Combine(_env.WebRootPath, "images");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // Chuyển tên dịch vụ thành slug (bỏ dấu, viết thường, gạch nối)
            string slug = tenDichVu
                .ToLower()
                .Normalize(System.Text.NormalizationForm.FormD)
                .Replace("[\\u0300-\\u036f]", "") // bỏ dấu
                .Replace("đ", "d") // riêng chữ đ
                .Replace(" ", "-")
                .Replace("[^a-z0-9-]", ""); // chỉ giữ a-z, số, -

            string extension = Path.GetExtension(file.FileName);
            string fileName = $"{slug}{extension}";
            string filePath = Path.Combine(uploadsFolder, fileName);

            // Nếu trùng tên ảnh thì thêm hậu tố chuỗi ngẫu nhiên
            while (System.IO.File.Exists(filePath))
            {
                string randomSuffix = Path.GetRandomFileName().Substring(0, 6); // ví dụ: 'abc123'
                fileName = $"{slug}_{randomSuffix}{extension}";
                filePath = Path.Combine(uploadsFolder, fileName);
            }

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Ok(new { fileName });
        }
        private bool DichVuExists(int id)
        {
            return _context.DichVus.Any(e => e.DichVuID == id);
        }
        [HttpPost("import")]
        public async Task<IActionResult> ImportDichVu(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Chưa chọn file.");

            if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                return BadRequest("File không hợp lệ, chỉ hỗ trợ .xlsx");

            var dichVuList = new List<DichVu>();

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var workbook = new XLWorkbook(stream))
                {
                    var worksheet = workbook.Worksheet("DichVu");
                    if (worksheet == null)
                        return BadRequest("Không tìm thấy sheet tên 'DichVu'.");

                    foreach (var row in worksheet.RowsUsed().Skip(1)) 
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
                            var loaiDichVuID = row.Cell(8).GetValue<int>();

                            
                            if (string.IsNullOrEmpty(tenDichVu) || gia < 0 || thoiGian <= 0)
                                continue;

                            var dv = new DichVu
                            {
                                TenDichVu = tenDichVu,
                                Gia = gia,
                                ThoiGian = thoiGian,
                                MoTa = moTa,
                                HinhAnh = hinhAnh,
                                TrangThai = trangThai,
                                NgayTao = ngayTao,
                                LoaiDichVuID = loaiDichVuID
                            };

                            dichVuList.Add(dv);
                        }
                        catch (Exception ex)
                        {
                            continue;
                        }
                    }
                }
            }

            _context.DichVus.AddRange(dichVuList);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, count = dichVuList.Count });
        }

    }
}
