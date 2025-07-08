namespace ProductCatalog.Application.DTOs
{
    public class ProductQueryDto
    {
        public string? Name { get; set; }
        public string? Brand { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}


