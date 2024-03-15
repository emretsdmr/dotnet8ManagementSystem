using Microsoft.AspNetCore.Identity;

namespace ManagementSystem_DotNet8.Entities
{
    public class User : IdentityUser
    {
        public virtual ICollection<Information> Informations { get; set; }
    }
}
