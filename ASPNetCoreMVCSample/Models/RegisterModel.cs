using System.ComponentModel.DataAnnotations;

namespace ASPNetCoreMVCSample.Models
{
    public class RegisterModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [StringLength(100, ErrorMessage ="Invalid Password lenght",MinimumLength =6)]
        public string Password { get; set; }
        [Required]
        [Compare("Password", ErrorMessage ="Password do not Match")]
        public string ConfirmPassword { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
    }
}
