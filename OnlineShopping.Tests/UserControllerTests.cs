using Microsoft.AspNetCore.Http;
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
using System.Security.Claims;
using System.Threading.Tasks;

namespace OnlineShopping.Tests
{
    [ExcludeFromCodeCoverage]
    public class UserControllerTests
    {
        private DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("onlineShopping")
            .Options;

        private ApplicationDbContext _context;

        private UsersController controller;

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


        [SetUp]
        public void Setup()
        {
            _context = new ApplicationDbContext(options);
            SeedDatabase();
            controller = new UsersController(_context);
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
                _context.Users.AddRange(users);
                _context.SaveChanges();
            }
        }

        [Test]
        public async Task AuthenticatedUserDataAccessReturnOkRequestAndUserData()
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

            var result = await controller.GetMe() as ObjectResult;


            Assert.That(result?.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value.GetType(), Is.EqualTo(typeof(User)));
        }

        [Test]
        public async Task InvalidUserAccessGetMeReturnBadRequestAndErrorMessage()
        {
            //Arrange
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(
                        new Claim[] {
                            new Claim("Id", "32"),
                            new Claim(ClaimTypes.Name, "ramkumar"),
                            new Claim(ClaimTypes.GivenName, "Ramkumar"),
                            new Claim(ClaimTypes.Role, "Admin")
                        }, "TestAuthentication"
                    )
                );
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            var result = await controller.GetMe() as ObjectResult;
            var errorMessage = result.Value as ErrorResponse;

            Assert.That(result?.StatusCode, Is.EqualTo(400));
            Assert.AreEqual(errorMessage.Message, "No user found");
        }

        [Test]
        public async Task PostUserUserDataIsNullReturnsBadRequest()
        {
            UserDto? newUser = null;

            var result = await controller.PostUser(newUser) as ObjectResult;
            var errorMessage = result.Value as ErrorResponse;

            Assert.That(result?.StatusCode, Is.EqualTo(400));
            Assert.AreEqual(errorMessage.Message, "Data empty");
        }

        [Test]
        public async Task PostUserUserDataWithExistingUsernameReturnsBadRequest()
        {
            UserDto? newUser = new UserDto
            {
                Name = "Ramkumar",
                EmailAddress = "ramkumarmani2000@gmail.com",
                About = "Full stack developer",
                City = "Tuty",
                Username = "ramkumar",
                Password = "Ramkumar@45",
            };

            var result = await controller.PostUser(newUser) as ObjectResult;
            var errorMessage = result.Value as ErrorResponse;

            Assert.That(result?.StatusCode, Is.EqualTo(400));
            Assert.AreEqual(errorMessage.Message, "Username already exists");
        }

        [Test]
        public async Task PostUserValidUserDataReturnsOkRequest()
        {
            UserDto? newUser = new UserDto
            {
                Name = "Jayarath",
                EmailAddress = "jayarath@gmail.com",
                About = "Full stack developer",
                City = "Chennai",
                Username = "jayarath",
                Password = "Ramkumar@45",
            };

            var result = await controller.PostUser(newUser) as ObjectResult;
            var response = result.Value as UserResponse;

            Assert.That(result?.StatusCode, Is.EqualTo(200));
            Assert.AreEqual(response.Message, "Profile created successfully");
        }

        [Test]
        public async Task UpdateUserDetailsWithInvalidAuthenticatedUserReturnBadRequestAndErrorMessage()
        {
            //Arrange
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(
                        new Claim[] {
                            new Claim("Id", "32"),
                            new Claim(ClaimTypes.Name, "ramkumar"),
                            new Claim(ClaimTypes.GivenName, "Ramkumar"),
                            new Claim(ClaimTypes.Role, "Admin")
                        }, "TestAuthentication"
                    )
                );
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            UserUpdateDto updatedUserDetails = new UserUpdateDto
            {
                Name = "Ramkumar",
                About = "Full stack developer",
                EmailAddress = "ramkumar@gmail.com",
                City = "Tuty"
            };

            var result = await controller.PutUser(updatedUserDetails) as ObjectResult;
            var errorMessage = result.Value as ErrorResponse;

            Assert.That(result?.StatusCode, Is.EqualTo(400));
            Assert.AreEqual(errorMessage.Message, "No user found");
        }

        [Test]
        public async Task UpdateUserDetailsWithValidAuthenticatedUserReturnBadRequestAndErrorMessage()
        {
            //Arrange
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(
                        new Claim[] {
                            new Claim("Id", "1"),
                            new Claim(ClaimTypes.Name, "ramkumar"),
                            new Claim(ClaimTypes.GivenName, "Ramkumar"),
                            new Claim(ClaimTypes.Role, "Admin")
                        }, "TestAuthentication"
                    )
                );
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            UserUpdateDto updatedUserDetails = new UserUpdateDto
            {
                Name = "Ramkumar",
                About = "Full stack developer",
                EmailAddress = "ramkumar@gmail.com",
                City = "Tuty"
            };

            var result = await controller.PutUser(updatedUserDetails) as ObjectResult;
            var response = result.Value as UserResponse;

            Assert.That(result?.StatusCode, Is.EqualTo(200));
            Assert.AreEqual(response.Message, "Profile updated");
        }

        [Test]
        public async Task UpdateUserPasswordWithInvalidAuthenticatedUserReturnBadRequestAndErrorMessage()
        {
            //Arrange
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(
                        new Claim[] {
                            new Claim("Id", "32"),
                            new Claim(ClaimTypes.Name, "ramkumar"),
                            new Claim(ClaimTypes.GivenName, "Ramkumar"),
                            new Claim(ClaimTypes.Role, "Admin")
                        }, "TestAuthentication"
                    )
                );
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            PasswordUpdateDto updatedUserPassword = new PasswordUpdateDto
            {
                OldPassword = "Ramkumar@45",
                NewPassword = "Ramkumar@55",
                ConfirmPassword = "Ramkumar@55"
            };

            var result = await controller.UpdatePassword(updatedUserPassword) as ObjectResult;
            var errorMessage = result.Value as ErrorResponse;

            Assert.That(result?.StatusCode, Is.EqualTo(400));
            Assert.AreEqual(errorMessage.Message, "No user found");
        }

        [Test]
        public async Task UpdatePasswordWithValidAuthenticatedUserButNewPasswordIsNotCorrectReturnBadRequestAndErrorMessage()
        {
            //Arrange
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(
                        new Claim[] {
                            new Claim("Id", "1"),
                            new Claim(ClaimTypes.Name, "ramkumar"),
                            new Claim(ClaimTypes.GivenName, "Ramkumar"),
                            new Claim(ClaimTypes.Role, "Admin")
                        }, "TestAuthentication"
                    )
                );
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            PasswordUpdateDto updatedUserPassword = new PasswordUpdateDto
            {
                OldPassword = "Ramkumar@45",
                NewPassword = "Ramkumar@55",
                ConfirmPassword = "Ramkumar@54"
            };

            var result = await controller.UpdatePassword(updatedUserPassword) as ObjectResult;
            var errorMessage = result.Value as ErrorResponse;

            Assert.That(result?.StatusCode, Is.EqualTo(400));
            Assert.AreEqual(errorMessage.Message, "Password doesn't match. ReType");
        }

        [Test]
        public async Task UpdatePasswordWithValidAuthenticatedUserButOldPasswordIsNotCorrectReturnBadRequestAndErrorMessage()
        {
            //Arrange
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(
                        new Claim[] {
                            new Claim("Id", "1"),
                            new Claim(ClaimTypes.Name, "ramkumar"),
                            new Claim(ClaimTypes.GivenName, "Ramkumar"),
                            new Claim(ClaimTypes.Role, "Admin")
                        }, "TestAuthentication"
                    )
                );
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            PasswordUpdateDto updatedUserPassword = new PasswordUpdateDto
            {
                OldPassword = "Ramkumar@55",
                NewPassword = "Ramkumar@55",
                ConfirmPassword = "Ramkumar@55"
            };

            var result = await controller.UpdatePassword(updatedUserPassword) as ObjectResult;
            var errorMessage = result.Value as ErrorResponse;

            Assert.That(result?.StatusCode, Is.EqualTo(400));
            Assert.AreEqual(errorMessage.Message, "Old password is incorrect");
        }

        [Test]
        public async Task UpdatePasswordWithValidAuthenticatedUserWithValidDataReturnOkRequest()
        {
            //Arrange
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(
                        new Claim[] {
                            new Claim("Id", "1"),
                            new Claim(ClaimTypes.Name, "ramkumar"),
                            new Claim(ClaimTypes.GivenName, "Ramkumar"),
                            new Claim(ClaimTypes.Role, "Admin")
                        }, "TestAuthentication"
                    )
                );
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            PasswordUpdateDto updatedUserPassword = new PasswordUpdateDto
            {
                OldPassword = "Ramkumar@45",
                NewPassword = "Ramkumar@55",
                ConfirmPassword = "Ramkumar@55"
            };

            var result = await controller.UpdatePassword(updatedUserPassword) as ObjectResult;
            var response = result.Value as UserResponse;

            Assert.That(result?.StatusCode, Is.EqualTo(200));
            Assert.AreEqual(response.Message, "Password Updated");
        }

        [Test]
        public async Task DeleteUserWithValidAuthenticatedUserReturnOkRequest()
        {
            //Arrange
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(
                        new Claim[] {
                            new Claim("Id", "1"),
                            new Claim(ClaimTypes.Name, "ramkumar"),
                            new Claim(ClaimTypes.GivenName, "Ramkumar"),
                            new Claim(ClaimTypes.Role, "Admin")
                        }, "TestAuthentication"
                    )
                );
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            var result = await controller.DeleteUser() as ObjectResult;
            var response = result.Value as UserResponse;

            Assert.That(result?.StatusCode, Is.EqualTo(200));
            Assert.AreEqual(response.Message, "Deleted user");
        }

        [Test]
        public async Task DeleteUserWithInValidAuthenticatedUserReturnBadRequest()
        {
            //Arrange
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(
                        new Claim[] {
                            new Claim("Id", "2"),
                            new Claim(ClaimTypes.Name, "ramkumar"),
                            new Claim(ClaimTypes.GivenName, "Ramkumar"),
                            new Claim(ClaimTypes.Role, "Admin")
                        }, "TestAuthentication"
                    )
                );
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            var result = await controller.DeleteUser() as ObjectResult;
            var response = result.Value as ErrorResponse;

            Assert.That(result?.StatusCode, Is.EqualTo(400));
            Assert.AreEqual(response.Message, "No user found");
        }
    }
}
