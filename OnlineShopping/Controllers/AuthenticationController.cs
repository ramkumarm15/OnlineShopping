using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using OnlineShopping.Logger;
using OnlineShopping.Models;
using OnlineShopping.Models.DTO;

namespace OnlineShopping.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class AuthenticationController : ControllerBase
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly string _secretKey;
        private AuthenticationResponse response;
        private ErrorResponse errorResponse;

        public AuthenticationController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _secretKey = _configuration["Jwt:Key"];
            response = new AuthenticationResponse();
            errorResponse = new ErrorResponse();
        }

        /// <summary>
        /// Generate a JWT token for user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpPost]
        [Route("CreateToken")]
        public async Task<ActionResult> Token(Login? user)
        {
            if (!string.IsNullOrEmpty(user.Username) && !string.IsNullOrEmpty(user.Password))
            {

                Log.LogWrite(LogLevel.Information, $"Log In attempt for user : {user.Username}");
                if (_context.Users.Any(x => x.Username == user.Username && x.Password == user.Password))
                {
                    Log.LogWrite(LogLevel.Information, "User found");
                    User? userToBeLoggedIn = await _context.Users
                        .Where(x => x.Username == user.Username && x.Password == user.Password).FirstOrDefaultAsync();

                    string token = GenerateToken(user: userToBeLoggedIn);

                    response.AccessToken = token;
                    response.Username = userToBeLoggedIn.Username;
                    response.UserId = userToBeLoggedIn.Id;

                    return Ok(response);
                }

                Log.LogWrite(LogLevel.Warning, $"Log In attempt for unknown user from database: {user.Username}");

                errorResponse.Message = "Username and Password is incorrect. Try again";
                return BadRequest(errorResponse);

            }
            errorResponse.Message = "Username and Password is empty";
            return BadRequest(errorResponse);
        }

        private string GenerateToken(User user)
        {
            Log.LogWrite(LogLevel.Information, $"JWT Token creation for : {user.Id}");
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            List<Claim> claims = new List<Claim>
            {
                new Claim("Id", user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.GivenName, user.Name),
                new Claim(ClaimTypes.Role, user.Role)
            };

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
