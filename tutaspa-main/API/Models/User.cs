using Microsoft.AspNetCore.Identity;

namespace API.Models
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
        public string Address { get; set; } = "unknown"; 
        public bool FisrtLogin { get; set;  } = true ;
        public bool IsDeleted { get; internal set; }
    }
}
