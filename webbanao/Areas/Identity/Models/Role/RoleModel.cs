using Microsoft.AspNetCore.Identity;

namespace webbanao.Areas.Identity.Models.Role
{
    public class RoleModel : IdentityRole
    {
        public string[] Claims { get; set; }

    }
}
