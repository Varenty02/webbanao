using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel;
using webbanao.Models;

namespace webbanao.Areas.Identity.Models.User
{
    public class AddUserRoleModel
    {
        public AppUser user { get; set; }

        [DisplayName("Các role gán cho user")]
        public string[] RoleNames { get; set; }

        public List<IdentityRoleClaim<string>> claimsInRole { get; set; }
        public List<IdentityUserClaim<string>> claimsInUserClaim { get; set; }

    }
}
