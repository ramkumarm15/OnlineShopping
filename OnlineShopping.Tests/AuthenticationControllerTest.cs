using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using OnlineShopping.Controllers;
using OnlineShopping.Models.DTO;
using OnlineShopping.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineShopping.Tests.Data;

namespace OnlineShopping.Tests
{
    public class AuthenticationControllerTest
    {
        private DbContextOptions<ApplicationDbContext> options 
            = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("onlineShopping")
                .Options;

        private ApplicationDbContext contextMock;
        private IConfiguration configuration;

        private AuthenticationController controller;


        public static IConfiguration getConfig()
        {
            var config = new ConfigurationBuilder()
              .SetBasePath("C:\\Users\\2107082\\source\\repos\\OnlineShopping\\OnlineShopping\\")
              .AddJsonFile("appsettings.json")
              .Build();
            return config;
        }

        [SetUp]
        public void Setup()
        {
            contextMock = new ApplicationDbContext(options);
            SeedDatabase();
            configuration = getConfig();
            controller = new AuthenticationController(contextMock, configuration);

        }

        [TearDown]
        public void Clear()
        {
            contextMock.Database.EnsureDeleted();
        }

        private void SeedDatabase()
        {
            contextMock.Database.EnsureCreated();

            if (!contextMock.Users.Any())
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
                };

                contextMock.AddRange(users);
                contextMock.SaveChanges();
            }
        }

        [Test]
        public async Task InputUsernameAndPasswordIsNullReturnsBadRequest()
        {
            // Arrange
            var loginData = Helper.GetLoginDetails();
            loginData.Username = null;
            loginData.Password = null;

            // Act
            var response = await controller.Token(loginData);
            var result = response as ObjectResult;
            var message = result.Value as ErrorResponse;

            // Assert
            Assert.That(result?.StatusCode, Is.EqualTo(400));
            Assert.That(typeof(ErrorResponse), Is.EqualTo(result.Value.GetType()));
            Assert.AreEqual(message.Message, "Username and Password is empty");
        }

        [Test]
        public async Task InValidInputUsernameAndPasswordReturnsBadRequest()
        {
            // Arrange
            var loginData = Helper.GetLoginDetails();
            loginData.Username = "Arun";
            loginData.Password = "Ramkumar";

            // Act
            var response = await controller.Token(loginData);
            var result = response as ObjectResult;
            var message = result.Value as ErrorResponse;

            // Assert
            Assert.That(result?.StatusCode, Is.EqualTo(400));
            Assert.AreEqual(message.Message, "Username and Password is incorrect. Try again");
        }

        [Test]
        public async Task ValidInputUsernameAndPasswordReturnsOkResultAndAccessToken()
        {
            // Arrange
            var loginData = Helper.GetLoginDetails();

            // Act
            var result = await controller.Token(loginData) as ObjectResult;
            var response = result.Value as AuthenticationResponse;

            // Assert
            Assert.That(result?.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value.GetType(), Is.EqualTo(typeof(AuthenticationResponse)));
        }
    }
}
