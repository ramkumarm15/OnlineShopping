using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.Models.DTO
{
    public class PasswordUpdateDto
    {
        [Required] public string OldPassword { get; set; }
        [Required] public string NewPassword { get; set; }
        [Required] public string ConfirmPassword { get; set; }
    }
}
