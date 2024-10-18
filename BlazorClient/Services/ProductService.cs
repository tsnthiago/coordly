using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using BlazorClient.Models;

namespace BlazorClient.Services
{
    public class ProductService
    {
        private readonly HttpClient _httpClient;

        public ProductService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ProductsResponse> GetProductsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/products");
                Console.WriteLine($"Response status: {response.StatusCode}");
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response content: {content}");
                response.EnsureSuccessStatusCode();

                var products = JsonSerializer.Deserialize<List<Product>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Product>();
                return new ProductsResponse { TotalCount = products.Count, Products = products };
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Error fetching products: {e.Message}");
                return new ProductsResponse();
            }
            catch (JsonException e)
            {
                Console.WriteLine($"Error deserializing response: {e.Message}");
                return new ProductsResponse();
            }
        }

        public async Task<Product> GetProductAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<Product>($"api/products/{id}") ?? new Product();
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/products", product);
                Console.WriteLine($"Response status: {response.StatusCode}");
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response content: {content}");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<Product>() ?? new Product();
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Error creating product: {e.Message}");
                // Considere lançar uma exceção personalizada ou retornar um resultado de erro
                throw;
            }
        }

        public async Task UpdateProductAsync(int id, Product product)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/products/{id}", product);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteProductAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/products/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
