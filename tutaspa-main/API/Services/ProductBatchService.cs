using API.Data;
using API.DTOs.Product;
using API.IService;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
	public class ProductBatchService : IProductBatchService
	{
		private readonly ApplicationDbContext _context;

		public ProductBatchService(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<bool> CreateBatchAsync(ProductBatchCreateDTO dto)
		{
			var product = await _context.Products
				.Include(p => p.ProductBatches)
				.FirstOrDefaultAsync(p => p.ProductId == dto.ProductId);

			if (product == null) return false;
			if (dto.ImportPrice >= product.CurrentSellingPrice)
			{
				return false;
			}
			string productCode = $"SP-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";

			var batch = new ProductBatch
			{
				ProductId = dto.ProductId,
				ProductCode = productCode,
				ImportPrice = dto.ImportPrice,
				Quantity = dto.Quantity,
				SupplierName = dto.SupplierName,
				ManufactureDate = dto.ManufactureDate,
				ExpiryDate = dto.ExpiryDate
			};
			var history = new InventoryHistory
			{
				ProductId = dto.ProductId,
				Batch = batch, // Gắn batch để có BatchId
				QuantityChanged = dto.Quantity,
				ActionType = "Import",
				Timestamp = DateTime.Now,
				SupplierName = dto.SupplierName,
				ImportPrice = dto.ImportPrice,
				
				ExpirationDate = dto.ExpiryDate,
				Note = "Nhập thêm lô hàng cho sản phẩm đã có"
			};
			_context.InventoryHistories.Add(history);
			_context.ProductBatches.Add(batch);
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<IEnumerable<ProductBatchDTO>> GetAllBatchesAsync()
		{
			return await _context.ProductBatches
				.Include(b => b.Product)
				.Select(b => new ProductBatchDTO
				{
					BatchId = b.BatchId,
					ProductId = b.ProductId,
					ProductName = b.Product.ProductName,
					ProductCode = b.ProductCode, // ✅ Lấy từ batch
					ImportPrice = b.ImportPrice,
					Quantity = b.Quantity,
					SupplierName = b.SupplierName,
					ManufactureDate = b.ManufactureDate,
					ExpiryDate = b.ExpiryDate
				})
				.ToListAsync();
		}

		public async Task<IEnumerable<ProductBatchDTO>> GetBatchesByProductAsync(int productId)
		{
			return await _context.ProductBatches
				.Include(b => b.Product)
				.Where(b => b.ProductId == productId)
				.Select(b => new ProductBatchDTO
				{
					BatchId = b.BatchId,
					ProductId = b.ProductId,
					ProductName = b.Product.ProductName,
					ProductCode = b.ProductCode, // ✅ Lấy từ batch
					ImportPrice = b.ImportPrice,
					Quantity = b.Quantity,
					SupplierName = b.SupplierName,
					ManufactureDate = b.ManufactureDate,
					ExpiryDate = b.ExpiryDate
				})
				.ToListAsync();
		}
	}
}
