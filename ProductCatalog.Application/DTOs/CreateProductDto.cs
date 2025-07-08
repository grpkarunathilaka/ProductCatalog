using System.ComponentModel.DataAnnotations;

namespace ProductCatalog.Application.DTOs
{
    public class CreateProductDto
    {
        public string Name { get; set; } = string.Empty;

        public string Brand { get; set; } = string.Empty;

        public decimal Price { get; set; }
    }
}


