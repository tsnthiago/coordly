using ProductAPI.Models;

public interface IProductRepository
{
    Task<Product> CreateAsync(Product product);
    Task<(IEnumerable<Product> Products, int TotalCount)> GetAllAsync(int pageNumber, int pageSize);
    Task<Product> GetByIdAsync(int id);
    Task UpdateAsync(int id, Product product);
    Task DeleteAsync(int id);
}
