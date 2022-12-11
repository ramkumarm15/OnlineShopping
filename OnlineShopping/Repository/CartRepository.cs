using Microsoft.EntityFrameworkCore;
using OnlineShopping.Models;
using OnlineShopping.Models.DTO;

namespace OnlineShopping.Repository
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _context;
        private CartResponse cartResponse;
        public CartRepository(ApplicationDbContext context)
        {
            _context = context;
            cartResponse= new CartResponse();
        }

        public async Task<CartResponse> Add(Cart cart, CartItemDto cartItem)
        {
            if (_context.CartItems.Any(ci => ci.Product.Id == cartItem.productId && ci.Cart.CartId == cart.CartId))
            {
                var response = await Update(cart, cartItem);

                return response;
            }

            Product? productToBeAddToCart = await _context.Products.
                                Where(p => p.Id == cartItem.productId).FirstOrDefaultAsync();

            // by default quantity is 1
            int cartItemQuantity = cartItem.quantity > 1 ? cartItem.quantity : 1;

            // quantity * product.Price
            double totalPriceOfCartItem = cartItemQuantity * productToBeAddToCart.Price;

            CartItems cartItemDb = new CartItems()
            {
                Cart = cart,
                Product = productToBeAddToCart,
                Quantity = cartItemQuantity,
                TotalPrice = totalPriceOfCartItem,
            };

            // update the all items price in cart table  
            cart.TotalPrice += totalPriceOfCartItem;

            cart.Updated = DateTime.Now;

            _context.CartItems.Add(cartItemDb);
            _context.Carts.Update(cart);
            await _context.SaveChangesAsync();

            cartResponse.Message = "Product added";

            return cartResponse;
        }
        public async Task<CartResponse> Update(Cart cart, CartItemDto cartItem)
        {
            Product? productToBeAddedToCart = await _context.Products
                                .Where(p => p.Id == cartItem.productId).FirstOrDefaultAsync();

            CartItems? cartItemOfCart = await _context.CartItems
                .Where(ci => ci.Cart.CartId == cart.CartId && ci.Product.Id == cartItem.productId).FirstOrDefaultAsync();

            // quantity * product.Price
            cartItemOfCart.Quantity = cartItem.quantity;

            // update the item price in cart item table  
            double totalPriceOfCartItem = cartItem.quantity * productToBeAddedToCart.Price;
            cartItemOfCart.TotalPrice = totalPriceOfCartItem;

            // update the all items price in cart table  
            double totalPriceOfCart = 0;
            List<CartItems> cartItemsFromCart = _context.CartItems
                .Where(ci => ci.Cart.CartId == cart.CartId).ToList();

            foreach (var cartItems in cartItemsFromCart)
            {
                totalPriceOfCart += cartItems.TotalPrice;
            }

            cart.TotalPrice = totalPriceOfCart;
            cart.Updated = DateTime.Now;

            _context.CartItems.Update(cartItemOfCart);
            _context.Carts.Update(cart);
            await _context.SaveChangesAsync();

            cartResponse.Message = "Product updated";

            return cartResponse;
        }
        public async Task<CartResponse> Remove(Cart cart, CartItemDto cartItem)
        {
            CartItems? cartItemOfCart = await _context.CartItems
                .Where(ci => ci.Cart.CartId == cart.CartId && ci.Product.Id == cartItem.productId).FirstOrDefaultAsync();

            cart.TotalPrice -= cartItemOfCart.TotalPrice;

            _context.CartItems.Remove(cartItemOfCart);
            _context.Carts.Update(cart);

            await _context.SaveChangesAsync();

            cartResponse.Message = "Product deleted";

            return cartResponse;
        }
    }
}
