using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using OnlineShopping.Controllers;
using OnlineShopping.Models;
using OnlineShopping.Models.DTO;
using OnlineShopping.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShopping.Tests
{
    public class CartItemRepositoryTest
    {
        private DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
.UseInMemoryDatabase("onlineShopping")
.Options;

        private ApplicationDbContext _context;

        private CartRepository repository;

        private CartItemController controller;

        private ClaimsPrincipal ValidUser, InValidUser, ValidUserWithCartDoestNotExists;

        [SetUp]
        public void Setup()
        {
            _context = new ApplicationDbContext(options);
            SeedDatabase();
            repository = new CartRepository(_context);
        }

        [TearDown]
        public void Clear()
        {
            _context.Database.EnsureDeleted();
        }

        private async void SeedDatabase()
        {
            _context.Database.EnsureCreated();

            if (!_context.Users.Any())
            {
                List<User> users = new List<User>
                {
                    new User
                    {
                        Id = 1,
                        Name = "Ramkumar",
                        EmailAddress = "ramkumarmani2000@gmail.com",
                        About = "Full stack developer",
                        City = "Tuty",
                        Username = "ramkumar",
                        Password = "Ramkumar@45",
                        Role = "Admin"
                    },
                    new User
                    {
                        Id = 2,
                        Name = "Arun",
                        EmailAddress = "arun@gmail.com",
                        About = "Full stack developer",
                        City = "Tuty",
                        Username = "arun",
                        Password = "Ramkumar@45",
                        Role = "User"
                    },
                };
                _context.Users.AddRange(users);
                _context.SaveChanges();
            }

            if (!_context.Carts.Any())
            {
                var user = _context.Users.FirstOrDefault(u => u.Id == 1);
                var user2 = _context.Users.FirstOrDefault(u => u.Id == 2);

                List<Cart> cart = new List<Cart>
                {
                    new Cart
                    {
                        CartId = 1,
                        TotalPrice = 0,
                        Created = DateTime.Now,
                        Updated = DateTime.Now,
                        User = user
                    },
                    new Cart
                    {
                        CartId = 2,
                        TotalPrice = 0,
                        Created = DateTime.Now,
                        Updated = DateTime.Now,
                        User = user2
                    }
                };

                _context.Carts.AddRange(cart);
                _context.SaveChanges();
            }

            if (!_context.Products.Any())
            {
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

                _context.AddRange(products);
                _context.SaveChanges();
            }

            if (!_context.CartItems.Any())
            {
                var cart = _context.Carts.FirstOrDefault(c => c.CartId == 1);
                var product = _context.Products.FirstOrDefault(p => p.Id == 1);

                CartItems cartItem = new CartItems
                {
                    CartItemsId = 1,
                    Cart = cart,
                    Product = product,
                    Quantity = 1,
                    TotalPrice = product.Price
                };

                cart.TotalPrice += cartItem.TotalPrice;

                _context.CartItems.Add(cartItem);
                _context.Carts.Update(cart);

                _context.SaveChanges();
            }
        }

        [Test]
        public async Task AddNewItemToCartReturnsActionMessage()
        {
            var cart = _context.Carts.FirstOrDefault(c => c.CartId == 2);
            CartItemDto payload = new CartItemDto
            {
                productId = 1,
                quantity = 1
            };

            var result = await repository.Add(cart,payload) as CartResponse;

            Assert.That(result.Message, Is.EqualTo("Product added"));
        }

        [Test]
        public async Task AddExistingItemToCartViaAddmethodReturnsActionMessage()
        {
            var cart = _context.Carts.FirstOrDefault(c => c.CartId == 1);
            CartItemDto payload = new CartItemDto
            {
                productId = 1,
                quantity = 1
            };

            var result = await repository.Add(cart, payload) as CartResponse;

            Assert.That(result.Message, Is.EqualTo("Product updated"));
        }

        [Test]
        public async Task AddExistingItemToCartReturnsActionMessage()
        {
            var cart = _context.Carts.FirstOrDefault(c => c.CartId == 1);
            CartItemDto payload = new CartItemDto
            {
                productId = 1,
                quantity = 1
            };

            var result = await repository.Update(cart, payload) as CartResponse;

            Assert.That(result.Message, Is.EqualTo("Product updated"));
        }

        [Test]
        public async Task RemoveItemFromCartReturnsActionMessage()
        {
            var cart = _context.Carts.FirstOrDefault(c => c.CartId == 1);
            CartItemDto payload = new CartItemDto
            {
                productId = 1,
                quantity = 1
            };

            var result = await repository.Remove(cart, payload) as CartResponse;

            Assert.That(result.Message, Is.EqualTo("Product deleted"));
        }
    }
}
