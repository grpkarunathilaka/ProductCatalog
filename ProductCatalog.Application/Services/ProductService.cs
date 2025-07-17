using Microsoft.Extensions.Logging;
using ProductCatalog.Application.DTOs;
using ProductCatalog.Application.Interfaces;
using ProductCatalog.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;

namespace ProductCatalog.Application.Services
{
    public class ProductService(IProductRepository productRepository, ILogger<ProductService> logger) : IProductService
    {
        private readonly IProductRepository _productRepository = productRepository;
        private readonly ILogger<ProductService> _logger = logger;
        public async Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto)
        {
            try
            {
                _logger.LogInformation("Creating product: {@CreateProductDto}", createProductDto);

                //Check for Duplicate product (Name + Brand combinatio)
                var exists = await _productRepository.ExistAsync(createProductDto.Name, createProductDto.Brand);
                if (exists) 
                    {
                        var errorMessage = $"Product with name '{createProductDto.Name}' and brand '{createProductDto.Brand}' already exists.";
                        _logger.LogWarning(errorMessage);
                        throw new InvalidOperationException(errorMessage);
                    }

                    var product = new Product
                    {
                        Name = createProductDto.Name,
                        Brand = createProductDto.Brand,
                        Price = createProductDto.Price,
                        CreateAt = DateTime.Now,
                        UpdateAt = DateTime.Now
                    };

                    var createdProduct = await _productRepository.CreateAsync(product);

                    _logger.LogInformation("Product created successfully with ID: {ProductId}", createdProduct.Id);

                    return MapToDto(createdProduct);
            }
            catch (Exception)
            {
                _logger.LogError("Error creating product: {@CreateProductDto}", createProductDto);
                throw;
            }
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            try
            {
                _logger.LogInformation("Deleting product with ID: {ProductId}", id);
                var result = await _productRepository.DeleteAsync(id);

                if (result)
                {
                    _logger.LogInformation("Product deleted successfully with ID:{ProductId}", id);
                }
                else
                {
                    _logger.LogWarning("Product with ID:{ProductId} not found for deletion", id);
                }

                return result;
            }
            catch (Exception)
            {
                _logger.LogError("Error deleting product with ID: {ProductId}", id);
                throw;
            }
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Getting Product by ID {ProductId}", id);

                var product = await _productRepository.GetByIdAsync(id);

                if (product == null) 
                    {
                        _logger.LogWarning("Product with ID:{ProductId} not found", id);
                        return null;
                    }

                
                return MapToDto(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product by ID {ProductId}", id);
                throw;
            }
        }
       

        public async Task<PageResult<ProductDto>> GetProductsAsync(ProductQueryDto query)
        {
            try
            {
                _logger.LogInformation("Getting products with query: {@Query}", query);
                var result = await _productRepository.GetAllAsync(query);

                return new PageResult<ProductDto>
                {
                    Items = result.Items.Select(MapToDto),
                    TotalCount = result.TotalCount,
                    PageNumber = result.PageNumber,
                    PageSize = result.PageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting products with query {@Query}", query);
                throw;
            }
        }

        public async Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto updateProductDto)
        {
            try
            {
                _logger.LogInformation("Updating product with ID: {ProductId}, Data: {@UpdateProductDto}",id, updateProductDto);

                var existingProduct = await _productRepository.GetByIdAsync(id);
                if (existingProduct == null)
                {
                    _logger.LogWarning("Product with ID: {ProductId} not found for update", id);
                    return null;
                }

                //Check for Duplicate product (Name + Brand combinatio) excluding current product
                var exists = await _productRepository.ExistAsync(updateProductDto.Name, updateProductDto.Brand, id);
                if (exists)
                {
                    var errorMessage = $"Product with name '{updateProductDto.Name}' and brand '{updateProductDto.Brand}' already exists.";
                    _logger.LogWarning(errorMessage);
                    throw new InvalidOperationException(errorMessage);
                }
                existingProduct.Name = updateProductDto.Name;
                existingProduct.Brand = updateProductDto.Brand;
                existingProduct.Price = updateProductDto.Price;
                existingProduct.UpdateAt = DateTime.UtcNow;

                var updateProduct = await _productRepository.UpdateAsync(existingProduct);

                _logger.LogInformation("Product updated successfully with ID: {ProductId}", updateProduct.Id);

                return MapToDto(updateProduct);
            }
            catch (Exception)
            {
                _logger.LogError("Error updating product with ID: {ProductId}", id);
                throw;
            }
        }

        #region Private Methods
        private static ProductDto MapToDto(Product product)
        {
            return new ProductDto(
               product.Id,
               product.Name,
               product.Brand,
               product.Price,
               product.CreateAt,
               product.UpdateAt
           );
        }
        #endregion
    }
}
