using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductCatalog.Application.DTOs
{
    public record ProductDto(
        int Id,
        string Name,
        string Brand,
        decimal Price,
        DateTime CreateAt,
        DateTime UpdateAt
    );
}


