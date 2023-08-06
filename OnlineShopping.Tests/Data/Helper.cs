using OnlineShopping.Models;
using OnlineShopping.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShopping.Tests.Data
{
    public static class Helper
    {
        public static ClaimsPrincipal GetValidUser()
        {
            return new ClaimsPrincipal(
                new ClaimsIdentity(
                    new Claim[] 
                    {
                        new Claim("Id", "1"),
                        new Claim(ClaimTypes.Name, "ramkumar"),
                        new Claim(ClaimTypes.GivenName, "Ramkumar"),
                        new Claim(ClaimTypes.Role, "Admin")
                    }, 
                    "TestAuthentication")
                );
        }
        public static ClaimsPrincipal GetInValidUser()
        {
            return new ClaimsPrincipal(
                new ClaimsIdentity(
                    new Claim[]
                    {
                        new Claim("Id", "6"),
                        new Claim(ClaimTypes.Name, "ramkumar"),
                        new Claim(ClaimTypes.GivenName, "Ramkumar"),
                        new Claim(ClaimTypes.Role, "Admin")
                    },
                    "TestAuthentication")
                );
        }

        public static Login GetLoginDetails()
        {
            return new Login()
            {
                Username = "ramkumar",
                Password = "Ramkumar@45"
            };
        }

        public static UserDto GetNewUser()
        {
            return new UserDto
            {
                Name = "Jayarath",
                EmailAddress = "jayarath@gmail.com",
                About = "Full stack developer",
                City = "Chennai",
                Username = "jayarath",
                Password = "Ramkumar@45",
            };
        }
    }
}
