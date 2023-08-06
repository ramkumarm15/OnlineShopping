using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using OnlineShopping.Controllers;
using OnlineShopping.Models;
using OnlineShopping.Models.DTO;
using OnlineShopping.Tests.Data;


namespace OnlineShopping.Tests
{
    public class BillingAddressControllerTest
    {
        private DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("onlineShopping")
            .Options;
        private ApplicationDbContext _context;
        private BillingAddressController controller;
        private List<BillingAddress> addresses;
        private List<User> users;

        [SetUp]
        public void Setup()
        {
            _context = new ApplicationDbContext(options);
            SeedDatabase();
            controller = new BillingAddressController(_context);
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
                        Name = "jayarath",
                        EmailAddress = "ramkumarmani2000@gmail.com",
                        About = "Full stack developer",
                        City = "Tuty",
                        Username = "ramkumar",
                        Password = "Ramkumar@45",
                        Role = "Admin"
                    },
                };
                _context.Users.AddRange(users);
                _context.SaveChanges();
            }

            if (!_context.BillingAddresses.Any())
            {
                var user = _context.Users.FirstOrDefault(x => x.Id == 1);
                addresses = new List<BillingAddress>
                {
                    new BillingAddress
                    {
                        Id=1,
                        BillingName="Ramkumar",
                        Address1 = "Test",
                        Address2 = "Test",
                        City= "Test",
                        State= "Test",
                        MobileNumber= 123456,
                        PostalCode = 123456,
                        User = user
                    }
                };
                _context.BillingAddresses.AddRange(addresses);
                _context.SaveChanges();
            }
        }

        [Test]
        public async Task GetListOfAddressWithInvalidUserIdReturnsBadRequest()
        {
            var user = Helper.GetInValidUser();
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            var result = await controller.GetBillingAddresses() as ObjectResult;
            var response = result.Value as BillingAddressResponse;

            Assert.That(result.StatusCode, Is.EqualTo(400));
            Assert.That(response.Message, Is.EqualTo("No address found"));
        }

        [Test]
        public async Task GetListOfAddressWithValidUserReturnsOkRequest()
        {
            var user = Helper.GetValidUser();
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            var result = await controller.GetBillingAddresses() as ObjectResult;

            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value.GetType(), Is.EqualTo(typeof(List<BillingAddress>)));
        }

        [Test]
        public async Task GetAddressWithInValidAddressIdInvalidUserIdReturnsBadRequest()
        {
            var user = Helper.GetInValidUser();
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
            int billingAddressId = 2;

            var result = await controller.GetBillingAddress(billingAddressId) as ObjectResult;
            var response = result.Value as BillingAddressResponse;

            Assert.That(result.StatusCode, Is.EqualTo(400));
            Assert.That(response.Message, Is.EqualTo("No address found"));
        }

        [Test]
        public async Task GetAddressWithValidAddressIdInvalidUserIdReturnsBadRequest()
        {
            var user = Helper.GetInValidUser();
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
            int billingAddressId = 1;

            var result = await controller.GetBillingAddress(billingAddressId) as ObjectResult;
            var response = result.Value as BillingAddressResponse;

            Assert.That(result.StatusCode, Is.EqualTo(400));
            Assert.That(response.Message, Is.EqualTo("No address found"));
        }

        [Test]
        public async Task GetAddressWithInValidAddressIdValidUserIdReturnsBadRequest()
        {
            var user = Helper.GetValidUser();
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
            int billingAddressId = 2;

            var result = await controller.GetBillingAddress(billingAddressId) as ObjectResult;
            var response = result.Value as BillingAddressResponse;

            Assert.That(result.StatusCode, Is.EqualTo(400));
            Assert.That(response.Message, Is.EqualTo("No address found"));
        }

        [Test]
        public async Task GetAddressWithValidAddressIdValidUserReturnsOkRequest()
        {
            var user = Helper.GetValidUser();
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
            int billingAddressId = 1;

            var result = await controller.GetBillingAddress(billingAddressId) as ObjectResult;

            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value.GetType(), Is.EqualTo(typeof(BillingAddress)));
        }

        [Test]
        public async Task UpdateAddressWithInValidAddressIdInvalidUserIdReturnsBadRequest()
        {
            var user = Helper.GetInValidUser();
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
            int billingAddressId = 2;
            BillingAddressDto updatedAddress = new BillingAddressDto
            {
                BillingName = "Ramkumar",
                Address1 = "Test",
                Address2 = "Test",
                City = "Test",
                State = "Test",
                MobileNumber = 123456,
                PostalCode = 123456,
            };

            var result = await controller.PutBillingAddress(billingAddressId, updatedAddress) as ObjectResult;
            var response = result.Value as BillingAddressResponse;

            Assert.That(result.StatusCode, Is.EqualTo(400));
            Assert.That(response.Message, Is.EqualTo("No address found"));
        }

        [Test]
        public async Task UpdateAddressWithInValidAddressIdValidUserIdReturnsBadRequest()
        {
            var user = Helper.GetValidUser();
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
            int billingAddressId = 2;

            BillingAddressDto updatedAddress = new BillingAddressDto
            {
                BillingName = "Ramkumar",
                Address1 = "Test",
                Address2 = "Test",
                City = "Test",
                State = "Test",
                MobileNumber = 123456,
                PostalCode = 123456,
            };

            var result = await controller.PutBillingAddress(billingAddressId, updatedAddress) as ObjectResult;
            var response = result.Value as BillingAddressResponse;

            Assert.That(result.StatusCode, Is.EqualTo(400));
            Assert.That(response.Message, Is.EqualTo("No address found"));
        }

        [Test]
        public async Task UpdateAddressWithValidAddressIdInValidUserIdReturnsBadRequest()
        {
            var user = Helper.GetInValidUser();
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
            int billingAddressId = 1;
            BillingAddressDto updatedAddress = new BillingAddressDto
            {
                BillingName = "Ramkumar",
                Address1 = "Test",
                Address2 = "Test",
                City = "Test",
                State = "Test",
                MobileNumber = 123456,
                PostalCode = 123456,
            };

            var result = await controller.PutBillingAddress(billingAddressId, updatedAddress) as ObjectResult;
            var response = result.Value as BillingAddressResponse;

            Assert.That(result.StatusCode, Is.EqualTo(400));
            Assert.That(response.Message, Is.EqualTo("No address found"));
        }

        [Test]
        public async Task UpdateAddressWithValidAddressIdValidUserReturnsOkRequest()
        {
            var user = Helper.GetValidUser();
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
            int billingAddressId = 1;
            BillingAddressDto updatedAddress = new BillingAddressDto
            {
                BillingName = "Ramkumar",
                Address1 = "Test",
                Address2 = "Test",
                City = "Test",
                State = "Test",
                MobileNumber = 123456,
                PostalCode = 123456,
            };

            var result = await controller.PutBillingAddress(billingAddressId, updatedAddress) as ObjectResult;
            var response = result.Value as BillingAddressResponse;

            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(response.Message, Is.EqualTo("Address updated"));
        }

        [Test]
        public async Task CreateAddressInvalidUserReturnsBadRequest()
        {
            var user = Helper.GetInValidUser();
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
            BillingAddressDto newAddress = null;

            var result = await controller.PostBillingAddress(newAddress) as ObjectResult;
            var response = result.Value as BillingAddressResponse;

            Assert.That(result.StatusCode, Is.EqualTo(400));
            Assert.That(response.Message, Is.EqualTo("Data empty"));
        }

        [Test]
        public async Task CreateAddressValidUserIdReturnsOkRequest()
        {
            var user = Helper.GetValidUser();
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
            BillingAddressDto newAddress = new BillingAddressDto
            {
                BillingName = "Ramkumar",
                Address1 = "Test",
                Address2 = "Test",
                City = "Test",
                State = "Test",
                MobileNumber = 123456,
                PostalCode = 123456,
            };

            var result = await controller.PostBillingAddress(newAddress) as ObjectResult;
            var response = result.Value as BillingAddressResponse;

            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(response.Message, Is.EqualTo("Address added"));
        }

        [Test]
        public async Task DeleteAddressWithInValidAddressIdInvalidUserIdReturnsBadRequest()
        {
            var user = Helper.GetInValidUser();
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
            int addressId = 2;

            var result = await controller.DeleteBillingAddress(addressId) as ObjectResult;
            var response = result.Value as BillingAddressResponse;

            Assert.That(result.StatusCode, Is.EqualTo(400));
            Assert.That(response.Message, Is.EqualTo("Address not found"));
        }

        [Test]
        public async Task DeleteAddressWithInValidAddressIdValidUserIdReturnsBadRequest()
        {
            var user = Helper.GetValidUser();
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
            int addressId = 2;

            var result = await controller.DeleteBillingAddress(addressId) as ObjectResult;
            var response = result.Value as BillingAddressResponse;

            Assert.That(result.StatusCode, Is.EqualTo(400));
            Assert.That(response.Message, Is.EqualTo("Address not found"));
        }

        [Test]
        public async Task DeleteAddressWithValidAddressIdInValidUserIdReturnsBadRequest()
        {
            var user = Helper.GetInValidUser();
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
            int addressId = 1;

            var result = await controller.DeleteBillingAddress(addressId) as ObjectResult;
            var response = result.Value as BillingAddressResponse;

            Assert.That(result.StatusCode, Is.EqualTo(400));
            Assert.That(response.Message, Is.EqualTo("Address not found"));
        }

        [Test]
        public async Task DeleteAddressWithValidAddressIdValidUserIdReturnsOkRequest()
        {
            var user = Helper.GetValidUser();
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
            int addressId = 1;

            var result = await controller.DeleteBillingAddress(addressId) as ObjectResult;
            var response = result.Value as BillingAddressResponse;

            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(response.Message, Is.EqualTo("Address deleted"));
        }
    }
}
