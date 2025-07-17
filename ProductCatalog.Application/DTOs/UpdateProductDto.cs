using System.ComponentModel.DataAnnotations;

namespace ProductCatalog.Application.DTOs
{
    public record UpdateProductDto
    (
        string Name = "",
        string Brand = "",
        decimal Price = 0m
    );
}


