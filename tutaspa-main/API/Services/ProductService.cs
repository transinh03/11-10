using API.Data;
using API.DTOs.Product;
using API.IService;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
	public class ProductService :	IProductService
	{
		private readonly ApplicationDbContext _context;
		private readonly IWebHostEnvironment _env;

		public ProductService(ApplicationDbContext context, IWebHostEnvironment env)
		{
			_context = context;
			_env = env;
		}

		public async Task<IEnumerable<ProductDTO>> GetAllAsync()
		{

			return await _context.Products
				.Include(p => p.ProductBatches)
				.Select(p => new ProductDTO
				{
					SanPhamId = p.ProductId,
					TenSP = p.ProductName,
					MoTa = p.Description,
					Gia = p.CurrentSellingPrice,
					SoLuong = p.ProductBatches
						.Where(b => b.ExpiryDate > DateTime.Now)
						.Sum(b => b.Quantity),
					LoaiSanPhamId = p.CategoryId,
					HinhAnh =  p.Images
				})
				.ToListAsync();
		}

		public async Task<ProductDTO> GetByIdAsync(int id)
		{
			var baseUrl = "https://tutaspa-api.onrender.com/images/";

			var product = await _context.Products
				.Include(p => p.ProductBatches)
				.FirstOrDefaultAsync(p => p.ProductId == id);

			if (product == null) return null;

			return new ProductDTO
			{
				SanPhamId = product.ProductId,
				TenSP = product.ProductName,
				MoTa = product.Description,
				Gia = product.CurrentSellingPrice,
				SoLuong = product.ProductBatches
					.Where(b => b.ExpiryDate > DateTime.Now)
					.Sum(b => b.Quantity),
				LoaiSanPhamId = product.CategoryId,
				HinhAnh = string.IsNullOrEmpty(product.Images) ? null : baseUrl + product.Images
			};
		}

		public async Task<ProductDTO> CreateAsync(ProductCreateDTO dto)
		{
			var baseUrl = "https://tutaspa-api.onrender.com/images/";
			var imageName = await SaveImageAsync(dto.Image);

			var product = new Product
			{
				ProductName = dto.ProductName,
				Description = dto.Description,
				CategoryId = dto.CategoryId,
				CurrentSellingPrice = dto.SellingPrice,
				Images = imageName,
				ProductBatches = new List<ProductBatch>
				{
					new ProductBatch
					{
						ImportPrice = dto.ImportPrice,
						Quantity = dto.Quantity,
						SupplierName = dto.SupplierName,
						ManufactureDate = dto.ManufactureDate,
						ExpiryDate = dto.ExpiryDate
					}
				}
			};

			_context.Products.Add(product);
			await _context.SaveChangesAsync();

			return new ProductDTO
			{
				SanPhamId = product.ProductId,
				TenSP = product.ProductName,
				Gia = product.CurrentSellingPrice,
				MoTa = product.Description,
				SoLuong = product.ProductBatches.Sum(b => b.Quantity),
				LoaiSanPhamId = product.CategoryId,
				HinhAnh = string.IsNullOrEmpty(product.Images) ? null : baseUrl + product.Images
			};
		}

		public async Task<bool> UpdateAsync(int id, ProductUpdateDTO dto)
		{
			var product = await _context.Products
				.FirstOrDefaultAsync(p => p.ProductId == id);

			if (product == null) return false;

			product.ProductName = dto.ProductName;
			product.Description = dto.Description;
			product.CategoryId = dto.CategoryId;
			product.CurrentSellingPrice = dto.SellingPrice;

			if (dto.Image != null)
			{
				var imageName = await SaveImageAsync(dto.Image);
				product.Images = imageName;
			}

			_context.Products.Update(product);
			await _context.SaveChangesAsync();
			return true;
		}


		public async Task<bool> DeleteAsync(int id)
		{
			var product = await _context.Products.FindAsync(id);
			if (product == null) return false;

			_context.Products.Remove(product);
			await _context.SaveChangesAsync();
			return true;
		}

		private async Task<string> SaveImageAsync(IFormFile image)
		{
			if (image == null || image.Length == 0) return null;

			var uploadsFolder = Path.Combine(_env.WebRootPath, "images");
			if (!Directory.Exists(uploadsFolder))
				Directory.CreateDirectory(uploadsFolder);

			var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
			var filePath = Path.Combine(uploadsFolder, fileName);

			using (var stream = new FileStream(filePath, FileMode.Create))
			{
				await image.CopyToAsync(stream);
			}

			return fileName;
		}

		public async Task<IEnumerable<ProductDTO>> SearchByNameAsync(string name)
		{
			var baseUrl = "https://tutaspa-api.onrender.com/images/";
			return await _context.Products
				.Include(p => p.ProductBatches)
				.Where(p => p.ProductName.Contains(name))
				.Select(p => new ProductDTO
				{
					SanPhamId = p.ProductId,
					TenSP = p.ProductName,
					Gia = p.CurrentSellingPrice,
					SoLuong = p.ProductBatches
						.Where(b => b.ExpiryDate > DateTime.Now)
						.Sum(b => b.Quantity),
					MoTa = p.Description,
					LoaiSanPhamId = p.CategoryId,
					HinhAnh = string.IsNullOrEmpty(p.Images) ? null : baseUrl + p.Images
				})
				.ToListAsync();
		}

		public async Task<IEnumerable<ProductDTO>> FilterByPriceAsync(decimal min, decimal max)
		{
			var baseUrl = "https://tutaspa-api.onrender.com/images/";
			return await _context.Products
				.Include(p => p.ProductBatches)
				.Where(p => p.CurrentSellingPrice >= min && p.CurrentSellingPrice <= max)
				.Select(p => new ProductDTO
				{
					SanPhamId = p.ProductId,
					TenSP = p.ProductName,
					Gia = p.CurrentSellingPrice,
					SoLuong = p.ProductBatches
						.Where(b => b.ExpiryDate > DateTime.Now)
						.Sum(b => b.Quantity),
					MoTa = p.Description,
					LoaiSanPhamId = p.CategoryId,
					HinhAnh = string.IsNullOrEmpty(p.Images) ? null : baseUrl + p.Images
				})
				.ToListAsync();
		}
        public async Task<object> GetPagedAsync(
     int page,
     int pageSize,
     string keyword = null,
     decimal? minPrice = null,
     decimal? maxPrice = null,
	 int? categoryId = null)
        {
            if (page <= 0 || pageSize <= 0)
                throw new ArgumentException("Page và PageSize phải lớn hơn 0");

            var baseUrl = "https://tutaspa-api.onrender.com/images/";

            // Bắt đầu query
            var query = _context.Products
                .Include(p => p.ProductBatches)
                .AsQueryable();

            // Filter theo tên sản phẩm
            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(p => p.ProductName.Contains(keyword));
            // 🔥 Filter theo loại sản phẩm
            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            // Filter theo giá
            if (minPrice.HasValue)
                query = query.Where(p => p.CurrentSellingPrice >= minPrice.Value);
            if (maxPrice.HasValue)
                query = query.Where(p => p.CurrentSellingPrice <= maxPrice.Value);

            // Chọn DTO
            var queryDto = query.Select(p => new ProductDTO
            {
                SanPhamId = p.ProductId,
                TenSP = p.ProductName,
                MoTa = p.Description,
                Gia = p.CurrentSellingPrice,
                SoLuong = p.ProductBatches
                            .Where(b => b.ExpiryDate > DateTime.Now)
                            .Sum(b => b.Quantity),
                LoaiSanPhamId = p.CategoryId,
                HinhAnh = string.IsNullOrEmpty(p.Images) ? null : baseUrl + p.Images
            });

            // Tổng số item sau filter
            var totalItems = await queryDto.CountAsync();

            // Phân trang
            var items = await queryDto
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
