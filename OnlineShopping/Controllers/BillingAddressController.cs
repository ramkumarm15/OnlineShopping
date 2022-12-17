using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShopping.Models;
using OnlineShopping.Models.DTO;

namespace OnlineShopping.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "User_Admin")]
    public class BillingAddressController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private BillingAddressResponse response;

        public BillingAddressController(ApplicationDbContext context)
        {
            _context = context;
            response = new BillingAddressResponse();
        }

        /// <summary>
        /// Get all Billing Address of user
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> GetBillingAddresses()
        {

            int userId = Convert.ToInt32(User.FindFirstValue("id"));


            List<BillingAddress> billingAddress = await _context.BillingAddresses
                .Where(x => x.User.Id == userId)
                .ToListAsync();
            if (billingAddress == null)
            {
                response.Message = "No address found";
                return BadRequest(response);
            }

            return Ok(billingAddress);

        }

        /// <summary>
        /// Get Specific Billing Address of user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetBillingAddress([FromRoute] int id)
        {
            if (BillingAddressExists(id))
            {

                int userId = Convert.ToInt32(User.FindFirstValue("id"));


                BillingAddress? billingAddress = await _context.BillingAddresses
                    .Where(x => x.Id == id && x.User.Id == userId)
                    .FirstOrDefaultAsync();


                return Ok(billingAddress);

            }
            response.Message = "No address found";
            return BadRequest(response);
        }


        /// <summary>
        /// Update Specific Billing address of user
        /// </summary>
        /// <param name="id"></param>
        /// <param name="billingAddress"></param>
        /// <returns></returns>
        [HttpPut("{id:int}")]
        public async Task<ActionResult> PutBillingAddress([FromRoute] int id,
            [FromBody] BillingAddressDto billingAddress)
        {

            if (BillingAddressExists(id))
            {
                int userId = Convert.ToInt32(User.FindFirstValue("id"));


                BillingAddress? billingAddressToBeUpdated = await _context.BillingAddresses
                    .Where(w => w.Id == id && w.User.Id == userId).FirstOrDefaultAsync();

                billingAddressToBeUpdated.BillingName = billingAddress.BillingName;
                billingAddressToBeUpdated.Address1 = billingAddress.Address1;
                billingAddressToBeUpdated.Address2 = billingAddress.Address2;
                billingAddressToBeUpdated.MobileNumber = billingAddress.MobileNumber;
                billingAddressToBeUpdated.City = billingAddress.City;
                billingAddressToBeUpdated.State = billingAddress.State;
                billingAddressToBeUpdated.PostalCode = billingAddress.PostalCode;


                _context.BillingAddresses.Update(billingAddressToBeUpdated);
                await _context.SaveChangesAsync();

                response.Message = "Address updated";
                return Ok(response);
            }

            response.Message = "Address not found";
            return BadRequest(response);
        }

        /// <summary>
        /// Create a new Billing Address for user
        /// </summary>
        /// <param name="billingAddress"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> PostBillingAddress([FromBody] BillingAddressDto billingAddress)
        {

            int userId = Convert.ToInt32(User.FindFirstValue("id"));


            User? userToAddBillingAddress = await _context.Users.FindAsync(userId);


            BillingAddress? billingAddressToBeAdded = new BillingAddress
            {
                BillingName = billingAddress.BillingName,
                Address1 = billingAddress.Address1,
                Address2 = billingAddress.Address2,
                City = billingAddress.City,
                State = billingAddress.State,
                PostalCode = billingAddress.PostalCode,
                MobileNumber = billingAddress.MobileNumber,
                User = userToAddBillingAddress
            };


            _context.BillingAddresses.Add(billingAddressToBeAdded);
            await _context.SaveChangesAsync();

            response.Message = "Address added";
            return Ok(response);

        }


        /// <summary>
        /// Delete Billing address of user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteBillingAddress([FromRoute] int id)
        {
            if (BillingAddressExists(id))
            {
                int userId = Convert.ToInt32(User.FindFirstValue("id"));


                BillingAddress? billingAddressToBeDeleted = await _context.BillingAddresses
                    .Where(w => w.Id == id && w.User.Id == userId).FirstOrDefaultAsync();


                _context.BillingAddresses.Remove(billingAddressToBeDeleted);
                await _context.SaveChangesAsync();

                response.Message = "Address deleted";
                return Ok(response);
            }

            response.Message = "Address not found";
            return BadRequest(response);
        }

        private bool BillingAddressExists(int id)
        {
            return _context.BillingAddresses.Any(e => e.Id == id);
        }
    }
}
