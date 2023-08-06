using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShopping.Models;
using OnlineShopping.Models.DTO;
using OnlineShopping.Repository;

namespace OnlineShopping.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "User_Admin")]
    public class CartItemController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ICartRepository _repository;
        private CartResponse cartResponse;

        public CartItemController(ApplicationDbContext context, ICartRepository repository)
        {
            _context = context;
            _repository = repository;
            cartResponse = new CartResponse();
        }

        /// <summary>
        /// Add product to cart
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> AddCartItem([FromBody] CartPayload payload)
        {
            int userId = Convert.ToInt32(User.FindFirstValue("id"));

            if(!_context.Users.Any(u=>u.Id == userId))
            {
                cartResponse.Message = "Unknown user. Cannot have access to add item to cart";
                return BadRequest(cartResponse);
            }

            Cart? cartOfUser = await _context.Carts
                    .Where(c => c.User.Id == userId).FirstOrDefaultAsync();


            if (payload.Operation.Equals(CartOperation.Add.ToLower()))
            {
                var response = await _repository.Add(cartOfUser, payload.Data);

                return Ok(response);
            }
            else if (payload.Operation.Equals(CartOperation.Update.ToLower()))
            {
                var response = await _repository.Update(cartOfUser, payload.Data);

                return Ok(response);
            }
            else if (payload.Operation.Equals(CartOperation.Delete.ToLower()))
            {
                var response = await _repository.Remove(cartOfUser, payload.Data);

                return Ok(response);
            }
            cartResponse.Message = "Cannot add product right now";
            return BadRequest(cartResponse);
        }
    }
}