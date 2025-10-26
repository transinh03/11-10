using API.DTOs.Product;

namespace API.IService
{
	public interface IInventoryService
	{
		Task<bool> ImportWithBatchAsync(ImportProductRequest dto);
		Task<bool> ExportWithBatchAsync(ExportProductRequest dto);
		Task<bool> UpdateSellingPriceAsync(UpdateSellingPriceRequest dto);
		Task<List<InventoryHistoryDTO>> GetHistoryAsync();
		Task<List<InventoryHistoryDTO>> GetImportHistoryAsync();
		Task<List<InventoryHistoryDTO>> GetExportHistoryAsync();
	}
}
