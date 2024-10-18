using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using ProductAPI.Models;
using Xunit;
using Xunit.Abstractions;
using System.Text.Json;

namespace ProductAPI.IntegrationTests
{
    public class ProductApiIntegrationTests
        : IClassFixture<CustomWebApplicationFactory<Program>>, IDisposable
    {
        private readonly HttpClient _client;
        private readonly List<TestOperation> _operations;
        private readonly ITestOutputHelper _output;

        public ProductApiIntegrationTests(
            CustomWebApplicationFactory<Program> factory,
            ITestOutputHelper output)
        {
            _client = factory.CreateClient();
            _operations = new List<TestOperation>();
            _output = output;
        }

        private void LogOperation(string operationType, string details)
        {
            _operations.Add(new TestOperation
            {
                OperationType = operationType,
                Details = details,
                Timestamp = DateTime.Now
            });
        }

        [Fact]
        public async Task GetAllProducts_ReturnsSuccessAndProducts()
        {
            // Act
            var response = await _client.GetAsync("/api/products");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ProductsResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            Assert.NotNull(result);
            Assert.NotNull(result.Products);
            Assert.IsType<List<Product>>(result.Products);

            LogOperation("GET", $"Retrieved {result.Products.Count} products");
        }

        [Fact]
        public async Task CreateProduct_ReturnsCreatedResponse()
        {
            // Arrange
            var newProduct = new Product { Name = "Test Product", Price = 9.99m, StockQuantity = 10 };

            // Act
            var response = await _client.PostAsJsonAsync("/api/products", newProduct);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var returnedProduct = await response.Content.ReadFromJsonAsync<Product>();
            Assert.NotNull(returnedProduct);
            Assert.Equal(newProduct.Name, returnedProduct!.Name);
            Assert.Equal(newProduct.Price, returnedProduct.Price);
            Assert.Equal(newProduct.StockQuantity, returnedProduct.StockQuantity);

            LogOperation("CREATE", $"Created product: ID={returnedProduct.ProductID}, Name={returnedProduct.Name}, Price={returnedProduct.Price}, StockQuantity={returnedProduct.StockQuantity}");
        }

        [Fact]
        public async Task GetProductById_ReturnsProduct()
        {
            // Arrange
            var newProduct = new Product { Name = "Get Test Product", Price = 19.99m, StockQuantity = 5 };
            var createResponse = await _client.PostAsJsonAsync("/api/products", newProduct);
            var createdProduct = await createResponse.Content.ReadFromJsonAsync<Product>();

            // Act
            var response = await _client.GetAsync($"/api/products/{createdProduct!.ProductID}");

            // Assert
            response.EnsureSuccessStatusCode();
            var returnedProduct = await response.Content.ReadFromJsonAsync<Product>();
            Assert.NotNull(returnedProduct);
            Assert.Equal(createdProduct.ProductID, returnedProduct!.ProductID);
            Assert.Equal(newProduct.Name, returnedProduct.Name);

            LogOperation("GET", $"Retrieved product: ID={returnedProduct.ProductID}, Name={returnedProduct.Name}, Price={returnedProduct.Price}, StockQuantity={returnedProduct.StockQuantity}");
        }

        [Fact]
        public async Task UpdateProduct_ReturnsNoContent()
        {
            // Arrange
            var newProduct = new Product { Name = "Update Test Product", Price = 29.99m, StockQuantity = 15 };
            var createResponse = await _client.PostAsJsonAsync("/api/products", newProduct);
            var createdProduct = await createResponse.Content.ReadFromJsonAsync<Product>();

            var updatedProduct = new Product
            {
                ProductID = createdProduct!.ProductID,
                Name = "Updated Product",
                Price = 39.99m,
                StockQuantity = 20
            };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/products/{createdProduct.ProductID}", updatedProduct);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            // Verify the update
            var getResponse = await _client.GetAsync($"/api/products/{createdProduct.ProductID}");
            var returnedProduct = await getResponse.Content.ReadFromJsonAsync<Product>();
            Assert.NotNull(returnedProduct);
            Assert.Equal(updatedProduct.Name, returnedProduct!.Name);
            Assert.Equal(updatedProduct.Price, returnedProduct.Price);
            Assert.Equal(updatedProduct.StockQuantity, returnedProduct.StockQuantity);

            LogOperation("UPDATE", $"Updated product: ID={returnedProduct.ProductID}, Name={returnedProduct.Name}, Price={returnedProduct.Price}, StockQuantity={returnedProduct.StockQuantity}");
        }

        [Fact]
        public async Task DeleteProduct_ReturnsNoContent()
        {
            // Arrange
            var newProduct = new Product { Name = "Delete Test Product", Price = 49.99m, StockQuantity = 25 };
            var createResponse = await _client.PostAsJsonAsync("/api/products", newProduct);
            var createdProduct = await createResponse.Content.ReadFromJsonAsync<Product>();

            // Act
            var response = await _client.DeleteAsync($"/api/products/{createdProduct!.ProductID}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            // Verify the delete
            var getResponse = await _client.GetAsync($"/api/products/{createdProduct.ProductID}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);

            LogOperation("DELETE", $"Deleted product: ID={createdProduct.ProductID}, Name={createdProduct.Name}, Price={createdProduct.Price}, StockQuantity={createdProduct.StockQuantity}");
        }

        [Fact]
        public async Task GetNonExistentProduct_ReturnsNotFound()
        {
            // Act
            var response = await _client.GetAsync("/api/products/999999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            LogOperation("GET", "Attempted to retrieve non-existent product (ID: 999999)");
        }

        [Fact]
        public async Task CreateInvalidProduct_ReturnsBadRequest()
        {
            // Arrange
            var invalidProduct = new Product { Name = "", Price = -10m, StockQuantity = -5 };

            // Act
            var response = await _client.PostAsJsonAsync("/api/products", invalidProduct);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            LogOperation("CREATE", $"Attempted to create invalid product: Name='{invalidProduct.Name}', Price={invalidProduct.Price}, StockQuantity={invalidProduct.StockQuantity}");
        }

        [Fact]
        public async Task UpdateNonExistentProduct_ReturnsNotFound()
        {
            // Arrange
            var nonExistentProduct = new Product { ProductID = 999999, Name = "Non-existent", Price = 9.99m, StockQuantity = 10 };

            // Act
            var response = await _client.PutAsJsonAsync("/api/products/999999", nonExistentProduct);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            LogOperation("UPDATE", $"Attempted to update non-existent product: ID={nonExistentProduct.ProductID}, Name={nonExistentProduct.Name}");
        }

        [Fact]
        public async Task DeleteNonExistentProduct_ReturnsNotFound()
        {
            // Act
            var response = await _client.DeleteAsync("/api/products/999999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            LogOperation("DELETE", "Attempted to delete non-existent product (ID: 999999)");
        }

        // New tests for edge cases and model validations

        [Fact]
        public async Task CreateProductWithZeroPrice_ReturnsBadRequest()
        {
            // Arrange
            var productWithZeroPrice = new Product { Name = "Zero Price Product", Price = 0m, StockQuantity = 10 };

            // Act
            var response = await _client.PostAsJsonAsync("/api/products", productWithZeroPrice);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            LogOperation("CREATE", $"Attempted to create product with zero price: Name={productWithZeroPrice.Name}, Price={productWithZeroPrice.Price}");
        }

        [Fact]
        public async Task CreateProductWithNegativeStockQuantity_ReturnsBadRequest()
        {
            // Arrange
            var productWithNegativeStock = new Product { Name = "Negative Stock Product", Price = 9.99m, StockQuantity = -1 };

            // Act
            var response = await _client.PostAsJsonAsync("/api/products", productWithNegativeStock);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            LogOperation("CREATE", $"Attempted to create product with negative stock: Name={productWithNegativeStock.Name}, StockQuantity={productWithNegativeStock.StockQuantity}");
        }

        [Fact]
        public async Task CreateProductWithoutName_ReturnsBadRequest()
        {
            // Arrange
            var productWithoutName = new Product { Name = "", Price = 9.99m, StockQuantity = 10 };

            // Act
            var response = await _client.PostAsJsonAsync("/api/products", productWithoutName);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            LogOperation("CREATE", $"Attempted to create product without name: Price={productWithoutName.Price}, StockQuantity={productWithoutName.StockQuantity}");
        }

        [Fact]
        public async Task UpdateProductWithInvalidData_ReturnsBadRequest()
        {
            // Arrange
            var newProduct = new Product { Name = "Valid Product", Price = 29.99m, StockQuantity = 15 };
            var createResponse = await _client.PostAsJsonAsync("/api/products", newProduct);
            var createdProduct = await createResponse.Content.ReadFromJsonAsync<Product>();

            var invalidUpdate = new Product
            {
                ProductID = createdProduct!.ProductID,
                Name = "",
                Price = -1m,
                StockQuantity = -5
            };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/products/{createdProduct.ProductID}", invalidUpdate);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            LogOperation("UPDATE", $"Attempted to update product with invalid data: ID={invalidUpdate.ProductID}, Name='{invalidUpdate.Name}', Price={invalidUpdate.Price}, StockQuantity={invalidUpdate.StockQuantity}");
        }

        public void Dispose()
        {
            GenerateReport();
        }

        private void GenerateReport()
        {
            var report = new StringBuilder();
            report.AppendLine("Test Operations Report");
            report.AppendLine("======================");
            report.AppendLine();

            foreach (var operation in _operations)
            {
                report.AppendLine($"{operation.Timestamp:yyyy-MM-dd HH:mm:ss} - {operation.OperationType}");
                report.AppendLine($"Details: {operation.Details}");
                report.AppendLine();
            }

            _output.WriteLine(report.ToString());
        }

        public class ProductsResponse
        {
            public List<Product>? Products { get; set; }
            public int CurrentPage { get; set; }
            public int PageSize { get; set; }
            public int TotalPages { get; set; }
            public int TotalCount { get; set; }
        }
    }
}
