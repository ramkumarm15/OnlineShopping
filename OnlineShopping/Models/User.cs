using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OnlineShopping.Models
{
    public enum Roles
    {
        Admin,
        User
    }

    public enum Gender
    {
        Male,
        Female,
        NotPreferToSay
    }

    public class User
    {
        [Key] public int Id { get; set; }
        [Required] public string Username { get; set; }

        [Required]
        [JsonIgnore]
        public string Password { get; set; }
        [Required] public string Name { get; set; }
        [Required] public string EmailAddress { get; set; }
        [Required] public string About { get; set; }

        [Required]
        [EnumDataType(typeof(Gender))]
        public string City { get; set; }
        [Required] public string Gender { get; set; }
        [Required] public int Age { get; set; }
        [Required] public double MobileNumber{ get; set; }

        [Required]
        [EnumDataType((typeof(Roles)))]
        public string Role { get; set; }

        public List<BillingAddress> BillingAddresses { get; set; }
        [JsonIgnore]
        public Cart Cart { get; set; }
    }
}
