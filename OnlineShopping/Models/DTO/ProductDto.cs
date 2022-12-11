using Microsoft.Build.Framework;

namespace OnlineShopping.Models.DTO
{
    public class ProductDto
    {
        [Required] public string Name { get; set; }
        [Required] public string Slug { get; set; }
        [Required] public string Description { get; set; }
        [Required] public double Price { get; set; }
        [Required] public string Image { get; set; }
    }
    
    public class ProductResponse
    {
        public string Message { get; set; }
    }
}