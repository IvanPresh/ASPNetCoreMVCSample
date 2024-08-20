using Microsoft.AspNetCore.Identity;

namespace ASPNetCoreMVCSample.Entities
{
    public class ApplicationUser:IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

    }
}
