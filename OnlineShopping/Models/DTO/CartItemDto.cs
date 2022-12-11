using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.Models.DTO
{
    public class CartOperation
    {
        public static readonly string Add = "add";
        public static readonly string Update = "update";
        public static readonly string Delete = "delete";
    }

    public class CartPayload
    {
        [Required]
        public CartItemDto Data { get; set; }

        [Required]
        public string Operation { get; set; }
    }

    public class CartItemDto
    {
        public int productId { get; set; }
        public int quantity { get; set; }
    }

    public class CartResponse
    {
        public string Message { get; set; }
    }
}