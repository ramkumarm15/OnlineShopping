using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OnlineShopping.Models
{
    public class CartItems
    {
        [Key] public int CartItemsId { get; set; }

        //public int ProductId { get; set; }
        public Product Product { get; set; }

        public int Quantity { get; set; }

        public double TotalPrice { get; set; }

        //[JsonIgnore] public int CartId { get; set; }
        [JsonIgnore] public Cart? Cart { get; set; }
    }
}

/*
 * Id(product),
 * quantity(optional, default=1),
 * totalPrice(quantity(default = 1) * product.price)
 * cart(id)
 */
