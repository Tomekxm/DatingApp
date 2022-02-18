using Microsoft.AspNetCore.Identity;

namespace API.Entities
{
    public class AppUserRole : IdentityUserRole<int>
    {
        public AppUser user { get; set; }
        public AppRole role { get; set; }
    }
}