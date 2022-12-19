using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShopping.Models;
using OnlineShopping.Models.DTO;

namespace OnlineShopping.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "User_Admin")]
    public class CartController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private ErrorResponse errorResponse;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
            errorResponse = new ErrorResponse();
        }

        /// <summary>
        /// Get cart of user
        /// </summary>
        /// <param name="cartId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> GetCart()
        {
            var userId = Convert.ToInt32(User.FindFirstValue("id"));

            if(!_context.Users.Any(u=>u.Id == userId))
            {
                errorResponse.Message = "User not found";
                return BadRequest(errorResponse);
            }

            var cart = await _context.Carts.Where(x => x.User.Id == userId)
                .Include(i => i.CartItemsList)
                .FirstOrDefaultAsync();

            if (cart == null)
            {
                User? userToCreateCart = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
                Cart cartForUser = new Cart()
                {
                    TotalPrice = 0,
                    Created = DateTime.Now,
                    Updated = DateTime.Now,
                    User = userToCreateCart
                };

                _context.Carts.Add(cartForUser);
                await _context.SaveChangesAsync();

                cart = cartForUser;
            }

            // Retrieve all cart items of this cart
            var cartItems = _context.CartItems.Where(x => x.Cart.CartId == cart.CartId)
                .Include(i => i.Product).ToList();
            cart.CartItemsList = cartItems;

            return Ok(cart);
        }

        /// <summary>
        /// Create new cart for specific user
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> CreateCart()
        {
            int userId = Convert.ToInt32(User.FindFirstValue("id"));

            if (_context.Users.Any(x => x.Id == userId))
            {
                if (!_context.Carts.Any(x => x.User.Id == userId))
                {
                    User? userToCreateCart = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
                    Cart cartForUser = new Cart()
                    {
                        TotalPrice = 0,
                        Created = DateTime.Now,
                        Updated = DateTime.Now,
                        User = userToCreateCart
                    };

                    _context.Carts.Add(cartForUser);
                    await _context.SaveChangesAsync();


                    return CreatedAtAction(nameof(GetCart), new { cartId = cartForUser.CartId }, cartForUser);
                }
                var cart = await _context.Carts.Where(x => x.User.Id == userId)
                    .Include(i => i.CartItemsList)
                    .FirstOrDefaultAsync();

                // Retrieve all cart items of this cart
                var cartItems = _context.CartItems.Where(x => x.Cart.CartId == cart.CartId)
                    .Include(i => i.Product).ToList();
                cart.CartItemsList = cartItems;

                return CreatedAtAction(nameof(GetCart), cart);
            }
            errorResponse.Message = "Cannot create cart for unknown user";
            return BadRequest(errorResponse);
        }
    }
}