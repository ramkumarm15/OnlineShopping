using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.OpenApi.Extensions;
using OnlineShopping.Models;
using OnlineShopping.Models.DTO;

namespace OnlineShopping.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "User_Admin")]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private UserResponse response;
        private ErrorResponse errorResponse;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
            response = new UserResponse();
            errorResponse = new ErrorResponse();
        }

        /// <summary>
        /// Get user by ID
        /// </summary>
        /// <returns></returns>
        [Route("GetMe")]
        [HttpGet]
        public async Task<IActionResult> GetMe()
        {
            int userId = Convert.ToInt32(User.FindFirstValue("id"));
            if (UserExists(userId))
            {
                User? user = await _context.Users.Where(x => x.Id == userId)
                    .FirstOrDefaultAsync();

                return Ok(user);
            }

            errorResponse.Message = "No user found";
            return BadRequest(errorResponse);
        }

        /// <summary>
        /// Create a new user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> PostUser([FromBody] UserDto? user)
        {
            if (!user.Equals(null))
            {
                if (UserExistsByUsername(user.Username))
                {
                    errorResponse.Message = "Username already exists";
                    return BadRequest(errorResponse);
                }

                User userMappingFromDto = new User
                {
                    Username = user.Username,
                    Password = user.Password,
                    Name = user.Name,
                    EmailAddress = user.EmailAddress,
                    About = user.About,
                    City = user.City,
                    Role = Roles.User.GetDisplayName()
                };

                Cart cartForUser = new Cart()
                {
                    TotalPrice = 0,
                    Created = DateTime.Now,
                    Updated = DateTime.Now,
                    User = userMappingFromDto
                };

                _context.Users.Add(userMappingFromDto);
                _context.Carts.Add(cartForUser);
                await _context.SaveChangesAsync();

                response.User = userMappingFromDto;
                response.Message = "Profile created successfully";
                return Ok(response);

            }

            errorResponse.Message = "Data empty";
            return BadRequest(errorResponse);
        }

        /// <summary>
        /// Update the user data
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [Route("update")]
        [HttpPut]
        public async Task<IActionResult> PutUser([FromBody] UserUpdateDto user)
        {
            int userId = Convert.ToInt32(User.FindFirstValue("id"));

            if (!UserExists(userId))
            {
                errorResponse.Message = "No user found";
                return BadRequest(errorResponse);
            }

            User? userToBeUpdated = await FindUser(userId);

            userToBeUpdated.Name = user.Name;
            userToBeUpdated.EmailAddress = user.EmailAddress;
            userToBeUpdated.About = user.About;
            userToBeUpdated.City = user.City;

            _context.Update(userToBeUpdated);
            await _context.SaveChangesAsync();

            response.User = userToBeUpdated;
            response.Message = "Profile updated";
            return Ok(response);
        }


        /// <summary>
        /// Update the user password
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [Route("update/password")]
        [HttpPatch]
        public async Task<IActionResult> UpdatePassword([FromBody] PasswordUpdateDto user)
        {
            int userId = Convert.ToInt32(User.FindFirstValue("id"));
            if (!UserExists(userId))
            {
                errorResponse.Message = "User not found";
                return BadRequest(errorResponse);
            }

            if (user.NewPassword == user.ConfirmPassword)
            {
                User userToBeUpdated = await FindUser(userId);

                if (userToBeUpdated.Password != user.OldPassword)
                {
                    errorResponse.Message = "Old password is incorrect";
                    return BadRequest(errorResponse);
                }

                userToBeUpdated.Password = user.NewPassword;

                _context.Users.Update(userToBeUpdated);
                await _context.SaveChangesAsync();

                response.Message = "Password Updated";
                return Ok(response);
            }
            else
            {
                errorResponse.Message = "Password doesn't match. ReType";
                return BadRequest(errorResponse);
            }
        }

        /// <summary>
        /// Delete the user from db
        /// </summary>
        /// <returns></returns>
        [Route("delete")]
        [HttpDelete]
        public async Task<IActionResult> DeleteUser()
        {
            int userId = Convert.ToInt32(User.FindFirstValue("id"));

            if (UserExists(userId))
            {
                User? userToBeDeleted = await FindUser(userId);

                _context.Users.Remove(userToBeDeleted);
                await _context.SaveChangesAsync();

                response.Message = "Deleted user";
                return Ok(response);
            }
            errorResponse.Message = "User not found";
            return BadRequest(errorResponse);
        }

        /// <summary>
        /// Check the user exists or not by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

        /// <summary>
        /// Check the user exists or not by username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        private bool UserExistsByUsername(string username)
        {
            return _context.Users.Any(e => e.Username == username);
        }

        private async Task<User?> FindUser(int id)
        {
            User? user = await _context.Users.FindAsync(id);
            return user;
        }
    }
}
