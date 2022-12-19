using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using OnlineShopping.Controllers;
using OnlineShopping.Models;
using OnlineShopping.Models.DTO;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShopping.Tests
{
    [ExcludeFromCodeCoverage]
    public class ProductControllerTest
    {
        private DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseInMemoryDatabase("onlineShopping")
        .Options;

        private ApplicationDbContext _context;

        private ProductController controller;

        List<Product> products = new List<Product>
        {
            new Product
            {
                Id = 1,
                Name= "Test",
                Slug = "test",
                Description= "Test",
                Price = 200,
                Image = "test image",
                IsActive= true,
                CreatedAt = DateTime.Now,
                UpdatedAt= DateTime.Now,
            }
        };


        [SetUp]
        public void Setup()
        {
            _context = new ApplicationDbContext(options);
            SeedDatabase();
            controller = new ProductController(_context);

        }

        [TearDown]
        public void Clear()
        {
            _context.Database.EnsureDeleted();
        }

        private void SeedDatabase()
        {
            _context.Database.EnsureCreated();

            if (!_context.Products.Any())
            {
                _context.AddRange(products);
                _context.SaveChanges();
            }
        }

        [Test, Order(1)]
        public async Task CreateProductWithValidIdReturnOkRequest()
        {
            int productId = 1;
            ProductDto newProduct = new ProductDto
            {
                Name = "Product 2",
                Slug = "Test",
                Description = "Test",
                Image = "Test",
                Price = 2000
            };

            var result = await controller.PostProduct(newProduct) as ObjectResult;
            var response = result.Value as ProductResponse;

            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(response.Message, Is.EqualTo("Product created"));
        }

        [Test, Order(2)]
        public async Task GetProductWithInvalidIdReturnsBadRequest()
        {
            int productId = 3;

            var result = await controller.GetProduct(productId) as ObjectResult;
            var response = result?.Value as ProductResponse;

            Assert.That(result.StatusCode, Is.EqualTo(400));
            Assert.That(response.Message, Is.EqualTo("Product not found"));
        }

        [Test, Order(3)]
        public async Task GetProductWithValidIdReturnOkRequest()
        {
            int productId = 1;


            var result = await controller.GetProduct(productId) as ObjectResult;

            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value.GetType(), Is.EqualTo(typeof(Product)));
        }

        [Test, Order(4)]
        public async Task UpdateProductWithInvalidIdReturnsBadRequest()
        {
            int productId = 8;
            ProductDto updateProduct = new ProductDto
            {
                Name = "Test",
                Slug = "Test",
                Description = "Test",
                Image = "Test",
                Price = 2000
            };

            var result = await controller.PutProduct(productId, updateProduct) as ObjectResult;
            var response = result?.Value as ProductResponse;

            Assert.That(result.StatusCode, Is.EqualTo(400));
            Assert.That(response.Message, Is.EqualTo("Product not found"));
        }

        [Test, Order(5)]
        public async Task UpdateProductWithValidIdReturnOkRequest()
        {
            int productId = 1;
            ProductDto updateProduct = new ProductDto
            {
                Name = "Test",
                Slug = "Test",
                Description = "Test",
                Image = "Test",
                Price = 2000
            };

            var result = await controller.PutProduct(productId, updateProduct) as ObjectResult;
            var response = result.Value as ProductResponse;

            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(response.Message, Is.EqualTo("Product updated"));
        }

        [Test, Order(6)]
        public async Task DeleteProductWithInvalidIdReturnsBadRequest()
        {
            int productId = 3;

            var result = await controller.DeleteProduct(productId) as ObjectResult;
            var response = result?.Value as ProductResponse;

            Assert.That(result.StatusCode, Is.EqualTo(400));
            Assert.That(response.Message, Is.EqualTo("Product not found"));
        }

        [Test, Order(7)]
        public async Task DeleteProductWithValidIdReturnOkRequest()
        {
            int productId = 1;


            var result = await controller.DeleteProduct(productId) as ObjectResult;
            var response = result?.Value as ProductResponse;

            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(response.Message, Is.EqualTo("Product deleted"));
        }

        [Test, Order(8)]
        public async Task GetAllProductsReturnsOkRequest()
        {
            var result = await controller.GetProducts() as ObjectResult;

            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value.GetType(), Is.EqualTo(typeof(List<Product>)));
        }

        [Test, Order(9)]
        public async Task GetAllProductsReturnsBadRequestAndNoProducts()
        {
            var data = _context.Products.ToList();
            _context.Products.RemoveRange(data);
            _context.SaveChanges();

            var result = await controller.GetProducts() as ObjectResult;
            var response = result.Value as ProductResponse;

            Assert.That(result.StatusCode, Is.EqualTo(400));
            Assert.That(response.Message, Is.EqualTo("Product not available right now"));
        }
    }
}
