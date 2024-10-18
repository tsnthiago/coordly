using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductAPI.Controllers;
using ProductAPI.Data;
using ProductAPI.Models;

namespace ProductAPI.Tests
{
    public class ProductsControllerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly ProductsController _controller;

        public ProductsControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);
            _controller = new ProductsController(_context);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task GetProducts_ReturnsAllProducts()
        {
            // Arrange
            _context.Products.AddRange(new List<Product>
            {
                new Product { Name = "Test Product 1", Price = 10.99m, StockQuantity = 100 },
                new Product { Name = "Test Product 2", Price = 20.99m, StockQuantity = 200 }
            });
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetProducts();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Product>>>(result);
            var returnValue = Assert.IsType<List<Product>>(actionResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task CreateProduct_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var newProduct = new Product { Name = "New Product", Price = 15.99m, StockQuantity = 50 };

            // Act
            var result = await _controller.CreateProduct(newProduct);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Product>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var returnValue = Assert.IsType<Product>(createdAtActionResult.Value);
            Assert.Equal(newProduct.Name, returnValue.Name);
        }

        [Fact]
        public async Task UpdateProduct_ReturnsNoContent()
        {
            // Arrange
            var existingProduct = new Product { Name = "Existing Product", Price = 10.99m, StockQuantity = 100 };
            _context.Products.Add(existingProduct);
            await _context.SaveChangesAsync();

            var updatedProduct = new Product 
            { 
                ProductID = existingProduct.ProductID, 
                Name = "Updated Product", 
                Price = 15.99m, 
                StockQuantity = 150 
            };

            // Act
            var result = await _controller.UpdateProduct(existingProduct.ProductID, updatedProduct);

            // Assert
            Assert.IsType<NoContentResult>(result);

            // Verify the update
            var productInDb = await _context.Products.FindAsync(existingProduct.ProductID);
            Assert.NotNull(productInDb);
            Assert.Equal(updatedProduct.Name, productInDb.Name);
            Assert.Equal(updatedProduct.Price, productInDb.Price);
            Assert.Equal(updatedProduct.StockQuantity, productInDb.StockQuantity);
        }

        [Fact]
        public async Task UpdateProduct_ReturnsBadRequest_WhenIdMismatch()
        {
            // Arrange
            var updatedProduct = new Product { ProductID = 2, Name = "Updated Product", Price = 15.99m, StockQuantity = 150 };

            // Act
            var result = await _controller.UpdateProduct(1, updatedProduct);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }
    }
}
