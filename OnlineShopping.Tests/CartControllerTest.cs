using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using OnlineShopping.Controllers;
using OnlineShopping.Models;
using OnlineShopping.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShopping.Tests
{
    public class CartControllerTest
    {
        private DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("onlineShopping")
            .Options;

        private ApplicationDbContext _context;

        private CartController controller;

        [SetUp]
        public void Setup()
        {
            _context = new ApplicationDbContext(options);
            SeedDatabase();
            controller = new CartController(_context);
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

                List<Cart> cart = new List<Cart>
                {
                    new Cart
                    {
                        CartId = 1,
                        TotalPrice = 0,
                        Created = DateTime.Now,
                        Updated = DateTime.Now,
                        User = user
                    }
                };

                _context.Carts.AddRange(cart);
                _context.SaveChanges();
            }
        }

        [Test]
        public async Task GetCartInvalidUserIdReturnsBadRequest()
        {
            //Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                        new Claim("Id", "12"),
                                        new Claim(ClaimTypes.Name, "ramkumar"),
                                        new Claim(ClaimTypes.GivenName, "Ramkumar"),
                                        new Claim(ClaimTypes.Role, "Admin")
                                   }, "TestAuthentication"));
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            //Act
            var result = await controller.GetCart() as ObjectResult;
            var response = result.Value as ErrorResponse;

            //Assert
            Assert.That(result.StatusCode, Is.EqualTo(400));
            Assert.That(response.Message, Is.EqualTo("User not found"));
        }

        [Test]
        public async Task GetCartVaildUserIdHasExistingCartReturnsOkRequest()
        {
            //Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                        new Claim("Id", "1"),
                                        new Claim(ClaimTypes.Name, "ramkumar"),
                                        new Claim(ClaimTypes.GivenName, "Ramkumar"),
                                        new Claim(ClaimTypes.Role, "Admin")
                                   }, "TestAuthentication"));
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            //Act
            var result = await controller.GetCart() as ObjectResult;

            //Assert
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value.GetType(), Is.EqualTo(typeof(Cart)));
        }

        [Test]
        public async Task GetCartVaildUserIdHasNoExistingCartAndCreateNewCartReturnsOkRequest()
        {
            //Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                        new Claim("Id", "2"),
                                        new Claim(ClaimTypes.Name, "ramkumar"),
                                        new Claim(ClaimTypes.GivenName, "Ramkumar"),
                                        new Claim(ClaimTypes.Role, "Admin")
                                   }, "TestAuthentication"));
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            //Act
            var result = await controller.GetCart() as ObjectResult;

            //Assert
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value.GetType(), Is.EqualTo(typeof(Cart)));
        }

        [Test]
        public async Task CreateCartInVaildUserIdReturnsBadRequest()
        {
            //Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                        new Claim("Id", "3"),
                                        new Claim(ClaimTypes.Name, "ramkumar"),
                                        new Claim(ClaimTypes.GivenName, "Ramkumar"),
                                        new Claim(ClaimTypes.Role, "Admin")
                                   }, "TestAuthentication"));
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            //Act
            var result = await controller.CreateCart() as ObjectResult;
            var response = result.Value as ErrorResponse;

            //Assert
            Assert.That(result.StatusCode, Is.EqualTo(400));
            Assert.That(response.Message, Is.EqualTo("Cannot create cart for unknown user"));
        }

        [Test]
        public async Task CreateCartVaildUserIdHasNoExistingCartAndCreateNewCartReturns201Request()
        {
            //Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                        new Claim("Id", "2"),
                                        new Claim(ClaimTypes.Name, "ramkumar"),
                                        new Claim(ClaimTypes.GivenName, "Ramkumar"),
                                        new Claim(ClaimTypes.Role, "Admin")
                                   }, "TestAuthentication"));
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            //Act
            var result = await controller.CreateCart() as ObjectResult;

            //Assert
            Assert.That(result.StatusCode, Is.EqualTo(201));
            Assert.That(result.Value.GetType(), Is.EqualTo(typeof(Cart)));
        }

        [Test]
        public async Task CreateCartVaildUserIdHasExistingCartReturns201RequestAndExistingCartDetails()
        {
            //Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                        new Claim("Id", "1"),
                                        new Claim(ClaimTypes.Name, "ramkumar"),
                                        new Claim(ClaimTypes.GivenName, "Ramkumar"),
                                        new Claim(ClaimTypes.Role, "Admin")
                                   }, "TestAuthentication"));
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            //Act
            var result = await controller.CreateCart() as ObjectResult;

            //Assert
            Assert.That(result.StatusCode, Is.EqualTo(201));
            Assert.That(result.Value.GetType(), Is.EqualTo(typeof(Cart)));
        }
    }
}
