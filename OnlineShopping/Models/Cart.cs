using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OnlineShopping.Models
{
    public class Cart
    {
        [Key] public int CartId { get; set; }

        public double TotalPrice { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public int UserId { get; set; }
        [JsonIgnore] public User User { get; set; }

        public List<CartItems> CartItemsList { get; set; }
    }
}

/*
 * Cart will be created if not exists
 * User(id, cart is unique to user[only one cart will be given for user])
 * TotalPrice(add all cartItems.totalPrice)
 */
