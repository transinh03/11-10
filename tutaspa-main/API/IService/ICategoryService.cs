using API.DTOs.Product;
namespace API.IService
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDTO>> GetAllAsync();
        Task<CategoryDTO> GetByIdAsync(int id);
        Task<CategoryDTO> CreateAsync(CategoryDTO dto);
        Task<bool> UpdateAsync(int id, CategoryDTO dto);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<CategoryDTO>> SearchByNameAsync(string ten);
        Task<object> GetPagedAsync(int page, int pageSize, string keyword = "");


    }
}
