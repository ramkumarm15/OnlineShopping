using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShopping.Models
{
    public class Product
    {
        [Key] public int Id { get; set; }
        [Column("Product_Name")] public string Name { get; set; }
        [Column("Product_Slug")] public string Slug { get; set; }
        [Column("Product_Description")] public string Description { get; set; }
        [Column("Product_Price")] public double Price { get; set; }
        [Column("Product_Image")] public string Image { get; set; }
        [Column("Product_IsActive")] public bool IsActive { get; set; }
        [Column("Product_Created")] public DateTime CreatedAt { get; set; }
        [Column("Product_Updated")] public DateTime UpdatedAt { get; set; }
    }
}