using Xunit;
using ProductCatalog.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductCatalog.Application.DTOs;
using ProductCatalog.Application.Interfaces;
using ProductCatalog.Domain.Models;
using Moq;
using Microsoft.Extensions.Logging;
using Assert = Xunit.Assert;

namespace ProductCatalog.Application.Services.Tests
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly Mock<ILogger<ProductService>> _loggerMock;
        private readonly ProductService _service;

        public ProductServiceTests()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _loggerMock = new Mock<ILogger<ProductService>>();
            _service = new ProductService(_productRepositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task CreateProductAsync_ShouldCreateProduct_WhenNotExists()
        {
            // Arrange
            var createDto = new CreateProductDto { Name = "Test", Brand = "Brand", Price = 10m };
            _productRepositoryMock.Setup(r => r.ExistAsync(createDto.Name, createDto.Brand, null)).ReturnsAsync(false);
            _productRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<Product>()))
                .ReturnsAsync((Product p) => { p.Id = 1; return p; });

            // Act
            var result = await _service.CreateProductAsync(createDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test", result.Name);
            Assert.Equal("Brand", result.Brand);
            Assert.Equal(10m, result.Price);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task CreateProductAsync_ShouldThrow_WhenDuplicateExists()
        {
            // Arrange
            var createDto = new CreateProductDto { Name = "Test", Brand = "Brand", Price = 10m };
            _productRepositoryMock.Setup(r => r.ExistAsync(createDto.Name, createDto.Brand, null)).ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateProductAsync(createDto));
        }

        [Fact]
        public async Task DeleteProductAsync_ShouldReturnTrue_WhenDeleted()
        {
            // Arrange
            _productRepositoryMock.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _service.DeleteProductAsync(1);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteProductAsync_ShouldReturnFalse_WhenNotFound()
        {
            // Arrange
            _productRepositoryMock.Setup(r => r.DeleteAsync(1)).ReturnsAsync(false);

            // Act
            var result = await _service.DeleteProductAsync(1);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetProductByIdAsync_ShouldReturnProduct_WhenExists()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "Test", Brand = "Brand", Price = 10m, CreateAt = DateTime.UtcNow, UpdateAt = DateTime.UtcNow };
            _productRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);

            // Act
            var result = await _service.GetProductByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task GetProductByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            // Arrange
            _productRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Product?)null);

            // Act
            var result = await _service.GetProductByIdAsync(1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetProductsAsync_ShouldReturnPagedResult()
        {
            // Arrange
            var query = new ProductQueryDto { PageNumber = 1, PageSize = 10 };
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Test", Brand = "Brand", Price = 10m, CreateAt = DateTime.UtcNow, UpdateAt = DateTime.UtcNow }
            };
            var pageResult = new PageResult<Product>
            {
                Items = products,
                TotalCount = 1,
                PageNumber = 1,
                PageSize = 10
            };
            _productRepositoryMock.Setup(r => r.GetAllAsync(query)).ReturnsAsync(pageResult);

            // Act
            var result = await _service.GetProductsAsync(query);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Items);
            Assert.Equal(1, result.TotalCount);
        }

        [Fact]
        public async Task UpdateProductAsync_ShouldUpdate_WhenValid()
        {
            // Arrange
            var updateDto = new UpdateProductDto { Name = "Updated", Brand = "Brand", Price = 20m };
            var existing = new Product { Id = 1, Name = "Test", Brand = "Brand", Price = 10m, CreateAt = DateTime.UtcNow, UpdateAt = DateTime.UtcNow };
            _productRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
            _productRepositoryMock.Setup(r => r.ExistAsync(updateDto.Name, updateDto.Brand, 1)).ReturnsAsync(false);
            _productRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Product>()))
                .ReturnsAsync((Product p) => p);

            // Act
            var result = await _service.UpdateProductAsync(1, updateDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated", result.Name);
            Assert.Equal(20m, result.Price);
        }

        [Fact]
        public async Task UpdateProductAsync_ShouldReturnNull_WhenNotFound()
        {
            // Arrange
            var updateDto = new UpdateProductDto { Name = "Updated", Brand = "Brand", Price = 20m };
            _productRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Product?)null);

            // Act
            var result = await _service.UpdateProductAsync(1, updateDto);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateProductAsync_ShouldThrow_WhenDuplicateExists()
        {
            // Arrange
            var updateDto = new UpdateProductDto { Name = "Updated", Brand = "Brand", Price = 20m };
            var existing = new Product { Id = 1, Name = "Test", Brand = "Brand", Price = 10m, CreateAt = DateTime.UtcNow, UpdateAt = DateTime.UtcNow };
            _productRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
            _productRepositoryMock.Setup(r => r.ExistAsync(updateDto.Name, updateDto.Brand, 1)).ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateProductAsync(1, updateDto));
        }
    }
}