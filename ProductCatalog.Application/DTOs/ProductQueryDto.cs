namespace ProductCatalog.Application.DTOs;

public record ProductQueryDto(
    string? Name = null,
    string? Brand = null,
    decimal? MinPrice = null,
    decimal? MaxPrice = null,
    int PageNumber = 1,
    int PageSize = 10
);


