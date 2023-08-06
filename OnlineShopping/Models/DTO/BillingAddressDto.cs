using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.Models.DTO
{
    /// <summary>
    /// Details get from user for Billing Address
    /// </summary>
    public class BillingAddressDto
    {
        [Required] public string BillingName { get; set; }
        [Required] public string Address1 { get; set; }
        [Required] public string Address2 { get; set; }
        [Required] public string City { get; set; }
        [Required] public string State { get; set; }
        [Required] public int PostalCode { get; set; }
        [Required] public double MobileNumber { get; set; }
        [Required] public bool defaultAddress { get; set; }
    }

    public class DefaultAddressDto
    {
        public int AddressId { get; set;}
    }
    public class BillingAddressResponse
    {
        public string Message { get; set; }
    }
}