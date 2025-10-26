using API.Data;
using API.DTOs.Product;
using API.IService;
using API.Models;
using Microsoft.EntityFrameworkCore;

public class InventoryService : IInventoryService
{
	private readonly ApplicationDbContext _context;
	private readonly IWebHostEnvironment _env;

	public InventoryService(ApplicationDbContext context, IWebHostEnvironment env)
	{
		_context = context;
		_env = env;
	}

	public async Task<bool> ImportWithBatchAsync(ImportProductRequest dto)
	{
		
		if (dto.Quantity <= 0)
		{
			return false; 
		}
		if (dto.ImportPrice <= 0 || dto.CurrentSellingPrice <= 0 || dto.CurrentSellingPrice <= dto.ImportPrice)
		{
			return false; 
		}
		
		var existingProduct = await _context.Products
			.FirstOrDefaultAsync(p => p.ProductName == dto.ProductName && p.CategoryId == dto.CategoryId);

		if (existingProduct != null)
		{
			
			return false; 
		}

		var product = new Product
		{
			ProductName = dto.ProductName,
			Description = dto.Description,
			CurrentSellingPrice = dto.CurrentSellingPrice,
			CategoryId = dto.CategoryId,
			Images = await SaveImageAsync(dto.Image),
			ProductBatches = new List<ProductBatch>()
		};

		_context.Products.Add(product);
		await _context.SaveChangesAsync();

		string productBatchCode = $"BATCH-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";

		var batch = new ProductBatch
		{
			ProductId = product.ProductId,
			ProductCode = productBatchCode,
			Quantity = dto.Quantity,
			SupplierName = dto.SupplierName,
			ImportPrice = dto.ImportPrice,
			ManufactureDate = dto.ManufactureDate,
			ExpiryDate = dto.ExpirationDate,
			CreatedAt = DateTime.Now
		};
		_context.ProductBatches.Add(batch);

		var history = new InventoryHistory
		{
			ProductId = product.ProductId,
			QuantityChanged = dto.Quantity,
			ActionType = "Import",
			Timestamp = DateTime.Now,
			SupplierName = dto.SupplierName,
			ImportPrice = dto.ImportPrice,
			ExpirationDate = dto.ExpirationDate,
			Note = string.IsNullOrEmpty(dto.Note) ? null : dto.Note,
			Batch = batch
		};
		_context.InventoryHistories.Add(history);

		await _context.SaveChangesAsync();
		return true;
	}

	public async Task<bool> ExportWithBatchAsync(ExportProductRequest dto)
	{
		var product = await _context.Products
			.Include(p => p.ProductBatches)
			.FirstOrDefaultAsync(p => p.ProductId == dto.ProductId);
		if (product == null || product.Quantity < dto.Quantity) return false;

		var batches = await _context.ProductBatches
			.Where(b => b.ProductId == dto.ProductId && b.Quantity > 0 && b.ExpiryDate > DateTime.Now)
			.OrderBy(b => b.ExpiryDate)
			.ToListAsync();

		int quantityToExport = dto.Quantity;

		foreach (var batch in batches)
		{
			if (quantityToExport <= 0) break;

			int take = Math.Min(batch.Quantity, quantityToExport);
			batch.Quantity -= take;
			quantityToExport -= take;

			var history = new InventoryHistory
			{
				ProductId = product.ProductId,
				QuantityChanged = -take,
				ActionType = "Export",
				Timestamp = DateTime.Now,
				SupplierName = batch.SupplierName,
				ImportPrice = batch.ImportPrice,
				ExpirationDate = batch.ExpiryDate,
				Note = dto.Note,
				Batch = batch
			};
			_context.InventoryHistories.Add(history);
		}

		if (quantityToExport > 0) return false;

		await _context.SaveChangesAsync();
		return true;
	}

	public async Task<List<InventoryHistoryDTO>> GetHistoryAsync()
	{
		return await _context.InventoryHistories
			.Include(h => h.Product)
			.Include(h => h.Batch)
			.OrderByDescending(h => h.Timestamp)
			.Select(h => new InventoryHistoryDTO
			{
				ProductId = h.ProductId,
				ProductName = h.Product.ProductName,
				ProductCode = h.Batch.ProductCode,
				BatchId = h.Batch != null ? h.Batch.BatchId : 0,
				ActionType = h.ActionType,
				QuantityChanged = h.QuantityChanged,
				Timestamp = h.Timestamp,
				SupplierName = h.SupplierName,
				ImportPrice = h.ImportPrice,
				ManufactureDate = h.Batch.ManufactureDate,
				ExpirationDate = h.ExpirationDate,
				Note = h.Note
			})
			.ToListAsync();
	}

	public async Task<List<InventoryHistoryDTO>> GetImportHistoryAsync()
	{
		return await _context.InventoryHistories
			.Include(h => h.Product)
			.Include(h => h.Batch)
			.Where(h => h.ActionType == "Import")
			.OrderByDescending(h => h.Timestamp)
			.Select(h => new InventoryHistoryDTO
			{
				ProductId = h.ProductId,
				ProductName = h.Product.ProductName,
				ProductCode = h.Batch.ProductCode,
				BatchId = h.Batch != null ? h.Batch.BatchId : 0,
				ActionType = h.ActionType,
				QuantityChanged = h.QuantityChanged,
				Timestamp = h.Timestamp,
				SupplierName = h.SupplierName,
				ImportPrice = h.ImportPrice,
				ManufactureDate = h.Batch.ManufactureDate,
				ExpirationDate = h.ExpirationDate,
				Note = h.Note
			})
			.ToListAsync();
	}

	public async Task<List<InventoryHistoryDTO>> GetExportHistoryAsync()
	{
		return await _context.InventoryHistories
			.Include(h => h.Product)
			.Include(h => h.Batch)
			.Where(h => h.ActionType == "Export")
			.OrderByDescending(h => h.Timestamp)
			.Select(h => new InventoryHistoryDTO
			{
				ProductId = h.ProductId,
				ProductName = h.Product.ProductName,
				ProductCode = h.Batch.ProductCode,
				BatchId = h.Batch != null ? h.Batch.BatchId : 0,
				ActionType = h.ActionType,
				QuantityChanged = h.QuantityChanged,
				Timestamp = h.Timestamp,
				SupplierName = h.SupplierName,
				ImportPrice = h.ImportPrice,
				ManufactureDate = h.Batch.ManufactureDate,
				ExpirationDate = h.ExpirationDate,
				Note = h.Note
			})
			.ToListAsync();
	}

	public async Task<bool> UpdateSellingPriceAsync(UpdateSellingPriceRequest dto)
	{
		var product = await _context.Products.FindAsync(dto.ProductId);
		if (product == null) return false;

		product.CurrentSellingPrice = dto.NewPrice;
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
}