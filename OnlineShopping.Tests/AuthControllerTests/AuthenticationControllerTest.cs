using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using OnlineShopping.Controllers;
using OnlineShopping.Models;
using OnlineShopping.Models.DTO;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShopping.Tests.AuthControllerTests
{
    [ExcludeFromCodeCoverage]
    public class AuthenticationControllerTest
    {
        private DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("onlineShopping")
            .Options;

        private ApplicationDbContext _context;
        private IConfiguration _config;

        private AuthenticationController controller;


        public static IConfiguration getConfig()
        {
            var config = new ConfigurationBuilder()
              .SetBasePath("D:\\Angular\\MentorTask\\OnlineShopping\\")
              .AddJsonFile("appsettings.json")
              .Build();
            return config;
        }

        [SetUp]
        public void Setup()
        {
            _context = new ApplicationDbContext(options);
            SeedDatabase();
            _config = getConfig();
            controller = new AuthenticationController(_context, _config);

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
                };

                _context.AddRange(users);
                _context.SaveChanges();
            }
        }

        [Test]
        public async Task InputUsernameAndPasswordIsNullReturnsBadRequest()
        {
            // Assign
            var input = new Login();
            input.Username = null;
            input.Password = null;

            // Arrange
            var response = await controller.Token(input);
            var result = response as ObjectResult;
            var message = result.Value as ErrorResponse;

            Assert.That(result?.StatusCode, Is.EqualTo(400));
            Assert.That(typeof(ErrorResponse), Is.EqualTo(result.Value.GetType()));
            Assert.AreEqual(message.Message, "Username and Password is empty");
        }

        [Test]
        public async Task InValidInputUsernameAndPasswordReturnsBadRequest()
        {
            // Assign
            var input = new Login();
            input.Username = "Arun";
            input.Password = "Ramkumar";

            // Arrange
            var response = await controller.Token(input);
            var result = response as ObjectResult;
            var message = result.Value as ErrorResponse;

            Assert.That(result?.StatusCode, Is.EqualTo(400));
            Assert.AreEqual(message.Message, "Username and Password is incorrect. Try again");
        }

        [Test]
        public async Task ValidInputUsernameAndPasswordReturnsOkResultAndAccessToken()
        {
            // Assign
            var loginData = new Login();
            loginData.Username = "ramkumar";
            loginData.Password = "Ramkumar@45";

            // Arrange
            var result = await controller.Token(loginData) as ObjectResult;
            var response = result.Value as AuthenticationResponse;

            // Act
            Assert.That(result?.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value.GetType(), Is.EqualTo(typeof(AuthenticationResponse)));
        }
    }
}
