using Microsoft.AspNetCore.Identity;

namespace GroceryOptimizerApi.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int UserId { get; set; }
    }
}