using Microsoft.VisualBasic.FileIO;
using ProductCatalog.Application.DTOs;
using ProductCatalog.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductCatalog.Application.Interfaces
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(int id);
        Task<PageResult<Product>> GetAllAsync(ProductQueryDto query);
        Task<Product> CreateAsync(Product product);
        Task<Product> UpdateAsync(Product product);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistAsync(string name, string brand, int? excluded = null);
    }
}
