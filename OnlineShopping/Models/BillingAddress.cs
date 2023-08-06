using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OnlineShopping.Models
{
    public class BillingAddress
    {
        [Key] public int Id { get; set; }
        [Required] public string BillingName { get; set; }
        [Required] public string Address1 { get; set; }
        [Required] public string Address2 { get; set; }
        [Required] public string City { get; set; }
        [Required] public string State { get; set; }
        [Required] public int PostalCode { get; set; }
        [Required] public double MobileNumber { get; set; }
        [Required] public bool Default { get; set; }
        [Required] public DateTime Created { get; set; }
        [Required] public DateTime Updated { get; set; }
        
        public int UserId { get; set; }
        [JsonIgnore] public User User { get; set; }
    }
}
