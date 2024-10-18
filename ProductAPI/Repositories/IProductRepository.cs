public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product> GetByIdAsync(int id);
    Task<Product> CreateAsync(Product product);
    Task UpdateAsync(int id, Product product);
    Task DeleteAsync(int id);
}
