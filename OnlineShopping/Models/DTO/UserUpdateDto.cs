using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.Models.DTO
{
    public class UserUpdateDto
    {
        [Required] public string Name { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }
        [Required] [MaxLength(200)] public string About { get; set; }
        [Required] public string City { get; set; }

        [Required]
        [EnumDataType(typeof(Gender))]
        public string Gender { get; set; }
        [Required] public int Age { get; set; }
        [Required] public double MobileNumber { get; set; }
    }
}
