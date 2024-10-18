using Microsoft.EntityFrameworkCore;
using ProductAPI.Models;
using System;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _context.Products.ToListAsync();
    }

    public async Task<Product> GetByIdAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            throw new KeyNotFoundException($"Product with ID {id} not found.");
        }
        return product;
    }

    public async Task<Product> CreateAsync(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task UpdateAsync(int id, Product product)
    {
        if (id != product.ProductID)
        {
            throw new ArgumentException("ID mismatch");
        }

        var existingProduct = await _context.Products.FindAsync(id);
        if (existingProduct == null)
        {
            throw new KeyNotFoundException($"Product with ID {id} not found.");
        }

        _context.Entry(existingProduct).CurrentValues.SetValues(product);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            throw new KeyNotFoundException($"Product with ID {id} not found.");
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
    }
}
