using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    public class CartItemControllerTest
    {
        private DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
    .UseInMemoryDatabase("onlineShopping")
    .Options;

        private ApplicationDbContext _context;

        private Mock<ICartRepository> repository;

        private CartItemController controller;

        private ClaimsPrincipal ValidUser, InValidUser, ValidUserWithCartDoestNotExists;

        [SetUp]
        public void Setup()
        {
            _context = new ApplicationDbContext(options);
            SeedDatabase();
            repository = new Mock<ICartRepository>();
            controller = new CartItemController(_context, repository.Object);
            ValidUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                        new Claim("Id", "1"),
                                        new Claim(ClaimTypes.Name, "ramkumar"),
                                        new Claim(ClaimTypes.GivenName, "Ramkumar"),
                                        new Claim(ClaimTypes.Role, "Admin")
                                   }, "TestAuthentication"));

            ValidUserWithCartDoestNotExists = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                        new Claim("Id", "2"),
                                        new Claim(ClaimTypes.Name, "ramkumar"),
                                        new Claim(ClaimTypes.GivenName, "Ramkumar"),
                                        new Claim(ClaimTypes.Role, "Admin")
                                   }, "TestAuthentication"));

            InValidUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                        new Claim("Id", "12"),
                                        new Claim(ClaimTypes.Name, "ramkumar"),
                                        new Claim(ClaimTypes.GivenName, "Ramkumar"),
                                        new Claim(ClaimTypes.Role, "Admin")
                                   }, "TestAuthentication"));
        }

        [TearDown]
        public void Clear()
        {
            _context.Database.EnsureDeleted();
        }

        private void SeedDatabase()
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
        }

        [Test]
        public async Task AddToCartInvalidUserReturnsBadRequest()
        {
            //Arrange
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = InValidUser }
            };
            var payload = new CartPayload
            {
                Operation = CartOperation.Add,
                Data = new CartItemDto
                {
                    productId = 1,
                }
            };

            //Act
            var result = await controller.AddCartItem(payload) as ObjectResult;
            var response = result.Value as CartResponse;

            //Assert
            Assert.That(result.StatusCode, Is.EqualTo(400));
            Assert.That(response.Message, Is.EqualTo("Unknown user. Cannot have access to add item to cart"));
        }

        [Test]
        public async Task AddToCartValidUserWithCartExistsReturnsOkRequest()
        {
            //Arrange
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = ValidUser }
            };
            var payload = new CartPayload
            {
                Operation = CartOperation.Add,
                Data = new CartItemDto
                {
                    productId = 1,
                }
            };

            var cart = await _context.Carts.FirstOrDefaultAsync(f => f.User.Id == 1);

            var cartRepoResponse = new CartResponse
            {
                Message = "Product added"
            };
            repository.Setup(s => s.Add(cart, payload.Data)).ReturnsAsync(cartRepoResponse);

            //Act
            var result = await controller.AddCartItem(payload) as ObjectResult;
            var response = result.Value as CartResponse;

            //Assert
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(response.Message, Is.EqualTo("Product added"));
        }

        [Test]
        public async Task UpdateCartItemValidUserReturnsOkRequest()
        {
            //Arrange
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = ValidUser }
            };
            var payload = new CartPayload
            {
                Operation = CartOperation.Update,
                Data = new CartItemDto
                {
                    productId = 1,
                }
            };

            var cart = await _context.Carts.FirstOrDefaultAsync(f => f.User.Id == 1);

            var cartRepoResponse = new CartResponse
            {
                Message = "Product updated"
            };
            repository.Setup(s => s.Update(cart, payload.Data)).ReturnsAsync(cartRepoResponse);

            //Act
            var result = await controller.AddCartItem(payload) as ObjectResult;
            var response = result.Value as CartResponse;

            //Assert
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(response.Message, Is.EqualTo("Product updated"));
        }

        [Test]
        public async Task DeleteCartItemValidUserReturnsOkRequest()
        {
            //Arrange
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = ValidUser }
            };
            var payload = new CartPayload
            {
                Operation = CartOperation.Delete,
                Data = new CartItemDto
                {
                    productId = 1,
                }
            };

            var cart = await _context.Carts.FirstOrDefaultAsync(f => f.User.Id == 1);

            var cartRepoResponse = new CartResponse
            {
                Message = "Product deleted"
            };
            repository.Setup(s => s.Remove(cart, payload.Data)).ReturnsAsync(cartRepoResponse);

            //Act
            var result = await controller.AddCartItem(payload) as ObjectResult;
            var response = result.Value as CartResponse;

            //Assert
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(response.Message, Is.EqualTo("Product deleted"));
        }

        [Test]
        public async Task UnknownCartOperationCartItemValidUserReturnsBadRequest()
        {
            //Arrange
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = ValidUser }
            };
            var payload = new CartPayload
            {
                Operation = "",
                Data = new CartItemDto
                {
                    productId = 1,
                }
            };

            var cart = await _context.Carts.FirstOrDefaultAsync(f => f.User.Id == 1);

            //Act
            var result = await controller.AddCartItem(payload) as ObjectResult;
            var response = result.Value as CartResponse;

            //Assert
            Assert.That(result.StatusCode, Is.EqualTo(400));
            Assert.That(response.Message, Is.EqualTo("Cannot add product right now"));
        }
    }
}
