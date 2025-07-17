using System.ComponentModel.DataAnnotations;

namespace ProductCatalog.Application.DTOs;

public record CreateProductDto(
 string Name = "",
 string Brand = "",
 decimal Price = 0m
);


