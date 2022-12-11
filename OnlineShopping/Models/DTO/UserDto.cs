using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.Models.DTO
{
    public class UserDto
    {
        [Required]
        [MaxLength(20), MinLength(8)]
        public string Username { get; set; }

        [Required]
        [MaxLength(20), MinLength(8)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        [Required]
        [MaxLength(200)]
        public string About { get; set; }

        [Required] public string City { get; set; }
    }

    public class AuthenticationResponse
    {
        public string AccessToken { get; set; }

        public string Username { get; set; }

        public int UserId { get; set; }
    }

    public class ErrorResponse
    {
        public string Message { get; set; }
    }

    public class UserResponse
    {
        public User User { get; set; }
        public string Message { get; set; }
    }
}
