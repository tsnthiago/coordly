namespace ProductAPI.Models
{
    public class ProductsResponse
    {
        public IEnumerable<Product> Products { get; set; } = default!;
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
    }
}
