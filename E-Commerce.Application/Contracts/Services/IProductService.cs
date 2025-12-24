using E_Commerce.Domain.Models;

namespace E_Commerce.Application.Contracts.Services
{
    public interface IProductService
    {
        Task<List<Product>> GetAllProductsAsync();
        Task<Product?> GetProductByIdAsync(string primary, string section, int id);
        Task<List<Product>> GetProductsByCategoryAsync(string primary, string section = null, string subcategory = null);
        Task<List<Product>> GetProductsByCollectionAsync(string collection);
        Task<Product> GetProductByIdAsync(string id);
        Task<List<FilterDefinition>> GetFiltersForCategoryAsync(string primary, string section, string subcategory, List<Product> products);
        List<Product> ApplyFilters(List<Product> products, Dictionary<string, List<string>> filters);
        List<Product> SortProducts(List<Product> products, string sortBy);
    }
}
