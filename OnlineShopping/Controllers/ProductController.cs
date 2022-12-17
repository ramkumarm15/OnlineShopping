using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using OnlineShopping.Models;
using OnlineShopping.Models.DTO;

namespace OnlineShopping.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Admin")]
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private ProductResponse response;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
            response = new ProductResponse();
        }

        /// <summary>
        /// Get all products
        /// </summary>
        /// <returns></returns>
        [HttpGet, AllowAnonymous]
        public async Task<ActionResult> GetProducts()
        {
            if (!_context.Products.Any())
            {
                response.Message = "Product not available right now";
                return BadRequest(response);
            }

            List<Product> products = await _context.Products.Where(x => x.IsActive).ToListAsync();
            return Ok(products);
        }

        /// <summary>
        /// Get products by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:int}"), AllowAnonymous]
        public async Task<ActionResult> GetProduct([FromRoute] int id)
        {
            if (ProductExists(id))
            {
                Product? product = await _context.Products.FindAsync(id);
                return Ok(product);
            }

            response.Message = "Product not found";
            return BadRequest(response);
        }

        /// <summary>
        /// Update product by ID
        /// </summary>
        /// <param name="id"></param>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPut("{id:int}")]
        public async Task<ActionResult> PutProduct([FromRoute] int id, [FromBody] ProductDto product)
        {
            if (ProductExists(id))
            {

                Product? productToBeUpdated = await _context.Products.FindAsync(id);

                productToBeUpdated.Name = product.Name;
                productToBeUpdated.Slug = product.Slug;
                productToBeUpdated.Price = product.Price;
                productToBeUpdated.Image = product.Image;
                productToBeUpdated.Description = product.Description;

                _context.Products.Update(productToBeUpdated);
                await _context.SaveChangesAsync();

                response.Message = "Product updated";
                return Ok(response);

            }

            response.Message = "Product not found";
            return BadRequest(response);
        }

        /// <summary>
        /// Create new product
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> PostProduct([FromBody] ProductDto product)
        {
            Product? newProduct = new Product();

            newProduct.Name = product.Name;
            newProduct.Slug = product.Slug;
            newProduct.Price = product.Price;
            newProduct.Image = product.Image;
            newProduct.Description = product.Description;
            newProduct.IsActive = true;
            newProduct.CreatedAt = DateTime.Now;
            newProduct.UpdatedAt = DateTime.Now;

            _context.Products.Add(newProduct);
            await _context.SaveChangesAsync();

            response.Message = "Product created";
            return Ok(response);
        }

        /// <summary>
        /// Delete product by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (ProductExists(id))
            {
                Product? productToBeDeleted = await _context.Products.FindAsync(id);

                _context.Products.Remove(productToBeDeleted);
                await _context.SaveChangesAsync();

                response.Message = "Product deleted";
                return Ok(response);
            }

            response.Message = "Product not found";
            return BadRequest(response);
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
