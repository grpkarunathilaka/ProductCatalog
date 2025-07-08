using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductCatalog.Application.DTOs;
using ProductCatalog.Application.Interfaces;
using ProductCatalog.Application.Services;
using ProductCatalog.Domain.Models;
using ProductCatalog.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductCatalog.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProductDbContext _context;
        private readonly ILogger<ProductRepository> _logger;

        public ProductRepository(ProductDbContext context, ILogger<ProductRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Product> CreateAsync(Product product)
        {
            try
            {
                _logger.LogInformation("Creating product: {@Product}", product);

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Product created successfully with ID: {ProductId}", product.Id);
                return product;
            }
            catch (Exception)
            {
                _logger.LogError("Error creating product: {@Product}", product);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                _logger.LogInformation("Deleting product with ID: {ProductId}", id);

                var product = await _context.Products.FindAsync(id);
                if (product == null) 
                {
                    _logger.LogWarning("Product with ID:{ProductId} not found for deletion", id);
                    return false;
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Product deleted successfully with ID:{ProductId}", id);
                return true;

            }
            catch (Exception)
            {
                _logger.LogError("Error deleting product with ID: {ProductId}", id);
                throw;
            }
        }

        public async Task<bool> ExistAsync(string name, string brand, int? excludeId = null)
        {
            try
            {
                _logger.LogInformation("Checking if product exists: Name={Name}, Brand={Brand}, Excluded={Excluded}", name, brand, excludeId);

                var query = _context.Products.Where(p => p.Name.ToLower() == name.ToLower() && p.Brand.ToLower() == brand.ToLower());

                if(excludeId.HasValue)
                {
                    query = query.Where(p => p.Id != excludeId.Value);
                }

                return await query.AnyAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if product exists: Name={Name}, Brand={Brand}, Excluded={Excluded}", name, brand, excludeId);
                throw;
            }
        }

        public async Task<PageResult<Product>> GetAllAsync(ProductQueryDto query)
        {
            try
            {
                _logger.LogInformation("Retrieving products with query: {@Query}", query);
                var queryable = _context.Products.AsQueryable();

                //Apply filters
                if (!string.IsNullOrEmpty(query.Name))
                {
                    queryable = queryable.Where(p => p.Name.Contains(query.Name));
                }

                if (!string.IsNullOrEmpty(query.Brand))
                {
                    queryable = queryable.Where(p => p.Brand.Contains(query.Brand));
                }

                if (query.MinPrice.HasValue)
                {
                    queryable = queryable.Where(p => p.Price >= query.MinPrice.Value);
                }
                if (query.MaxPrice.HasValue)
                {
                    queryable = queryable.Where(p => p.Price <= query.MaxPrice.Value);
                }

                //Get total count
                var totalCount = await queryable.CountAsync();

                //Apply pagination
                var items = await queryable
                    .OrderBy(p => p.Name)
                    .ThenBy(p => p.Brand)
                    .Skip((query.PageNumber - 1) * query.PageSize)
                    .Take(query.PageSize)
                    .ToListAsync();

                return new PageResult<Product>
                { 
                    PageSize = query.PageSize, 
                    TotalCount = totalCount, 
                    Items = items, 
                    PageNumber = query.PageNumber 
                };
            }
            catch (Exception)
            {
                _logger.LogInformation("Error retrieving products with query: {@Query}", query);
                throw;
            }
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Retrieving product with ID: {ProductId}", id);
                return await _context.Products.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving product with ID: {ProductId}", id);
                throw;
            }
        }

        public async Task<Product> UpdateAsync(Product product)
        {
            try
            {
                _logger.LogInformation("Updating product with ID: {ProductId}", product.Id);

                product.UpdateAt = DateTime.UtcNow;
                _context.Products.Update(product);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Product updated successfully with ID: {ProductId}", product.Id);
                return product;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error updating product with ID: {ProductId}", product.Id);
                throw;
            }
        }
    }
}
