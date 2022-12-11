using OnlineShopping.Models;
using OnlineShopping.Models.DTO;

namespace OnlineShopping.Repository
{
    public interface ICartRepository
    {
        Task<CartResponse> Add(Cart cart, CartItemDto cartItem);
        Task<CartResponse> Remove(Cart cart, CartItemDto cartItem);
        Task<CartResponse> Update(Cart cart, CartItemDto cartItem);
    }
}