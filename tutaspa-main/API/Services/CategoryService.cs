using API.Data;
using API.DTOs.Product;
using API.IService;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;

        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CategoryDTO>> GetAllAsync()
        {
            return await _context.Categories
                .Select(c => new CategoryDTO
                {
                    LoaiSanPhamId = c.CategoryId,
                    TenLoai = c.CategoryName,
                    maCategory = c.maCategory,
                }).ToListAsync();
        }

        public async Task<CategoryDTO> GetByIdAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return null;

            return new CategoryDTO
            {
                LoaiSanPhamId = category.CategoryId,
                TenLoai = category.CategoryName
            };
        }

        public async Task<CategoryDTO> CreateAsync(CategoryDTO dto)
        {
            var category = new Category
            {
                CategoryName = dto.TenLoai
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            dto.LoaiSanPhamId = category.CategoryId;
            return dto;
        }

        public async Task<bool> UpdateAsync(int id, CategoryDTO dto)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return false;

            category.CategoryName = dto.TenLoai;
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return false;

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<IEnumerable<CategoryDTO>> SearchByNameAsync(string ten)
        {
            return await _context.Categories
                .Where(c => c.CategoryName.Contains(ten))
                .Select(c => new CategoryDTO
                {
                    LoaiSanPhamId = c.CategoryId,
                    TenLoai = c.CategoryName
                }).ToListAsync();
        }
        public async Task<object> GetPagedAsync(int page, int pageSize, string keyword = "")
        {
            if (page <= 0 || pageSize <= 0)
                throw new ArgumentException("Page và PageSize phải lớn hơn 0");

            var query = _context.Categories.AsQueryable();

            // Tìm kiếm theo tên loại sản phẩm
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var lowerKeyword = keyword.Trim().ToLower();
                query = query.Where(c => c.CategoryName.ToLower().Contains(lowerKeyword));
            }

            var totalItems = await query.CountAsync();

            var items = await query
                .Select(c => new CategoryDTO
                {
                    LoaiSanPhamId = c.CategoryId,
                    TenLoai = c.CategoryName,
                    maCategory = c.maCategory
                })
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new
            {
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize,
                Items = items
            };
        }




    }
}
