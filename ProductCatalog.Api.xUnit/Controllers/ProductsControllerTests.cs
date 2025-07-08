using Xunit;
using ProductCatalog.Api.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Domain.Models;
using Microsoft.Extensions.Logging;
using ProductCatalog.Application.Interfaces;
using Moq;
using ProductCatalog.Application.DTOs;

namespace ProductCatalog.Api.Controllers.Tests
{
    public class ProductsControllerTests 

    {
        private readonly Mock<IProductService> _mockProductService;
        private readonly Mock<ILogger<ProductsController>> _mockLogger;
        private readonly ProductsController _controller;

        public ProductsControllerTests()
        {
            _mockProductService = new Mock<IProductService>();
            _mockLogger = new Mock<ILogger<ProductsController>>();
            _controller = new ProductsController(_mockProductService.Object, _mockLogger.Object);
        }

        private ProductDto CreateValidProduct(int? id = null)
        {
            var random = new Random();
            return new ProductDto
            {
                Id = id ?? random.Next(100),
                Name = "Controller Test Product",
                Brand = "Controller Test Brand",
                Price = 123.45m
            };
        }       
       

        [Fact]
        public async Task GetProduct_ReturnsOk_WhenProductExists()
        {
            // Arrange
            var random = new Random();
            int productId = random.Next(1, 1000); // Random product ID for testing
            var product = CreateValidProduct(productId);
            _mockProductService.Setup(s => s.GetProductByIdAsync(productId)).ReturnsAsync(product);

            // Act
            var result = await _controller.GetProduct(productId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(product, okResult.Value);
        }

        [Fact]
        public async Task GetProduct_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            _mockProductService.Setup(s => s.GetProductByIdAsync(2)).ReturnsAsync((ProductDto?)null);

            // Act
            var result = await _controller.GetProduct(2);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Contains("not found", notFoundResult.Value!.ToString(), System.StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task GetProduct_ReturnsServerError_WhenExceptionThrown()
        {
            // Arrange
            _mockProductService.Setup(s => s.GetProductByIdAsync(3)).ThrowsAsync(new System.Exception("DB error"));

            // Act
            var result = await _controller.GetProduct(3);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, objectResult.StatusCode);
            Assert.Contains("error", objectResult.Value!.ToString(), System.StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task GetProducts_ReturnsOk_WithPageResult()
        {
            // Arrange
            var query = new ProductQueryDto();           
            var product = CreateValidProduct();

            var pageResult = new PageResult<ProductDto>
            {
                Items = new List<ProductDto> { product },
                TotalCount = 1,
                PageNumber = 1,
                PageSize = 10
            };
            _mockProductService.Setup(s => s.GetProductsAsync(query)).ReturnsAsync(pageResult);

            // Act
            var result = await _controller.GetProducts(query);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(pageResult, okResult.Value);
        }

        [Fact]
        public async Task GetProducts_Returns500_OnException()
        {
            // Arrange
            var query = new ProductQueryDto();
            _mockProductService.Setup(s => s.GetProductsAsync(query)).ThrowsAsync(new Exception("fail"));

            // Act
            var result = await _controller.GetProducts(query);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, objectResult.StatusCode);
        }

        [Fact]
        public async Task CreateProduct_ReturnsCreatedAtAction_OnSuccess()
        {
            // Arrange
            var createDto = new CreateProductDto { Name = "Test", Brand = "Brand", Price = 10 };
            var productDto = CreateValidProduct();
            _mockProductService.Setup(s => s.CreateProductAsync(createDto)).ReturnsAsync(productDto);

            // Act
            var result = await _controller.CreateProduct(createDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(productDto, createdResult.Value);
        }        

        [Fact]
        public async Task CreateProduct_ReturnsConflict_OnDuplicate()
        {
            // Arrange
            var createDto = new CreateProductDto { Name = "Test", Brand = "Brand", Price = 10 };
            _mockProductService.Setup(s => s.CreateProductAsync(createDto)).ThrowsAsync(new InvalidOperationException("Duplicate"));

            // Act
            var result = await _controller.CreateProduct(createDto);

            // Assert
            var conflictResult = Assert.IsType<ConflictObjectResult>(result.Result);
        }

        [Fact]
        public async Task CreateProduct_Returns500_OnException()
        {
            // Arrange
            var createDto = new CreateProductDto { Name = "Test", Brand = "Brand", Price = 10 };
            _mockProductService.Setup(s => s.CreateProductAsync(createDto)).ThrowsAsync(new Exception("fail"));

            // Act
            var result = await _controller.CreateProduct(createDto);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, objectResult.StatusCode);
        }

        [Fact]
        public async Task UpdateProduct_ReturnsOk_OnSuccess()
        {
            // Arrange
            var updateDto = new UpdateProductDto { Name = "Test", Brand = "Brand", Price = 10 };
            var productDto = new ProductDto { Id = 1, Name = "Test", Brand = "Brand", Price = 10 };
            _mockProductService.Setup(s => s.UpdateProductAsync(1, updateDto)).ReturnsAsync(productDto);

            // Act
            var result = await _controller.UpdateProduct(1, updateDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(productDto, okResult.Value);
        }       

        [Fact]
        public async Task UpdateProduct_ReturnsNotFound_WhenProductNull()
        {
            // Arrange
            var updateDto = new UpdateProductDto { Name = "Test", Brand = "Brand", Price = 10 };
            _mockProductService.Setup(s => s.UpdateProductAsync(1, updateDto)).ReturnsAsync((ProductDto?)null);

            // Act
            var result = await _controller.UpdateProduct(1, updateDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task UpdateProduct_ReturnsConflict_OnDuplicate()
        {
            // Arrange
            var updateDto = new UpdateProductDto { Name = "Test", Brand = "Brand", Price = 10 };
            _mockProductService.Setup(s => s.UpdateProductAsync(1, updateDto)).ThrowsAsync(new InvalidOperationException("Duplicate"));

            // Act
            var result = await _controller.UpdateProduct(1, updateDto);

            // Assert
            var conflictResult = Assert.IsType<ConflictObjectResult>(result.Result);
        }

        [Fact]
        public async Task UpdateProduct_Returns500_OnException()
        {
            // Arrange
            var updateDto = new UpdateProductDto { Name = "Test", Brand = "Brand", Price = 10 };
            _mockProductService.Setup(s => s.UpdateProductAsync(1, updateDto)).ThrowsAsync(new Exception("fail"));

            // Act
            var result = await _controller.UpdateProduct(1, updateDto);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, objectResult.StatusCode);
        }

        [Fact]
        public async Task DeleteProduct_ReturnsNoContent_OnSuccess()
        {
            // Arrange
            _mockProductService.Setup(s => s.DeleteProductAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteProduct(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteProduct_ReturnsNotFound_WhenNotDeleted()
        {
            // Arrange
            _mockProductService.Setup(s => s.DeleteProductAsync(1)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteProduct(1);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task DeleteProduct_Returns500_OnException()
        {
            // Arrange
            _mockProductService.Setup(s => s.DeleteProductAsync(1)).ThrowsAsync(new Exception("fail"));

            // Act
            var result = await _controller.DeleteProduct(1);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
        }
    }
}