using System.Collections.Generic;

namespace BlazorClient.Models
{
    public class ProductsResponse
    {
        public int TotalCount { get; set; }
        public List<Product> Products { get; set; } = new List<Product>();
    }
}
