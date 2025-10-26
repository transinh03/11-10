using API.DTOs.Product;
namespace API.IService
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDTO>> GetAllAsync();
        Task<ProductDTO> GetByIdAsync(int id);
        Task<ProductDTO> CreateAsync(ProductCreateDTO dto);
        Task<bool> UpdateAsync(int id, ProductUpdateDTO dto);

        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<ProductDTO>> SearchByNameAsync(string name);
        Task<IEnumerable<ProductDTO>> FilterByPriceAsync(decimal min, decimal max);
        Task<object> GetPagedAsync(int page,
     int pageSize,
     string keyword = null,
     decimal? minPrice = null,
     decimal? maxPrice = null,
      int? categoryId = null);
    

    }
}
