using API.DTOs.Product;

namespace API.IService
{
	public interface IProductBatchService
	{
		Task<IEnumerable<ProductBatchDTO>> GetAllBatchesAsync();
		Task<IEnumerable<ProductBatchDTO>> GetBatchesByProductAsync(int productId);
		Task<bool> CreateBatchAsync(ProductBatchCreateDTO dto);
	}
}
