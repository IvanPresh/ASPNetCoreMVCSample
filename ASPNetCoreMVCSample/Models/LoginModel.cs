using System.ComponentModel.DataAnnotations;

namespace ASPNetCoreMVCSample.Models
{
    public class LoginModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [StringLength(maximumLength: 100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }    
    }
}
