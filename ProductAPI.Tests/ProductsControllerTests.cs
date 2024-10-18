using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ProductAPI.Controllers;
using ProductAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ProductAPI.Tests
{
    public class ProductsControllerTests
    {
        private readonly Mock<IProductRepository> _mockRepo;
        private readonly Mock<ILogger<ProductsController>> _mockLogger;
        private readonly ProductsController _controller;

        public ProductsControllerTests()
        {
            _mockRepo = new Mock<IProductRepository>();
            _mockLogger = new Mock<ILogger<ProductsController>>();
            _controller = new ProductsController(_mockRepo.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetProducts_ReturnsOkResult_WithListOfProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { ProductID = 1, Name = "Test Product 1", Price = 9.99m, StockQuantity = 10 },
                new Product { ProductID = 2, Name = "Test Product 2", Price = 19.99m, StockQuantity = 5 }
            };
            _mockRepo.Setup(repo => repo.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((products, products.Count));

            // Act
            var result = await _controller.GetProducts();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<ProductsResponse>(okResult.Value);

            Assert.NotNull(returnValue);
            Assert.IsType<List<Product>>(returnValue.Products);
            Assert.Equal(2, returnValue.Products.Count());
            Assert.Equal(1, returnValue.CurrentPage);
            Assert.Equal(10, returnValue.PageSize);
            Assert.Equal(1, returnValue.TotalPages);
            Assert.Equal(2, returnValue.TotalCount);
        }

        [Fact]
        public async Task GetProduct_ReturnsOkResult_WithProduct()
        {
            // Arrange
            var product = new Product { ProductID = 1, Name = "Test Product", Price = 9.99m, StockQuantity = 10 };
            _mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(product);

            // Act
            var result = await _controller.GetProduct(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedProduct = Assert.IsType<Product>(okResult.Value);
            Assert.Equal(product.ProductID, returnedProduct.ProductID);
        }

        [Fact]
        public async Task GetProduct_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetByIdAsync(1)).ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.GetProduct(1);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreateProduct_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var productToCreate = new Product { Name = "New Product", Price = 29.99m, StockQuantity = 15 };
            var createdProduct = new Product { ProductID = 3, Name = "New Product", Price = 29.99m, StockQuantity = 15 };
            _mockRepo.Setup(repo => repo.CreateAsync(It.IsAny<Product>())).ReturnsAsync(createdProduct);

            // Act
            var result = await _controller.CreateProduct(productToCreate);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedProduct = Assert.IsType<Product>(createdAtActionResult.Value);
            Assert.Equal(createdProduct.ProductID, returnedProduct.ProductID);
        }

        [Fact]
        public async Task UpdateProduct_ReturnsNoContent_WhenProductExists()
        {
            // Arrange
            var productToUpdate = new Product { ProductID = 1, Name = "Updated Product", Price = 39.99m, StockQuantity = 20 };
            _mockRepo.Setup(repo => repo.UpdateAsync(1, It.IsAny<Product>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateProduct(1, productToUpdate);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateProduct_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var productToUpdate = new Product { ProductID = 1, Name = "Updated Product", Price = 39.99m, StockQuantity = 20 };
            _mockRepo.Setup(repo => repo.UpdateAsync(1, It.IsAny<Product>())).ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.UpdateProduct(1, productToUpdate);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteProduct_ReturnsNoContent_WhenProductExists()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.DeleteAsync(1)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteProduct(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteProduct_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.DeleteAsync(1)).ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.DeleteProduct(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
