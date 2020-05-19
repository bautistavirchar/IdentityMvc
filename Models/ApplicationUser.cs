using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace IdentityMvc.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public string OldPassword { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
    }

    public class UserModel {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
    }

    public class ApplicationRole : IdentityRole {
        public string Description { get; set; }
    }
    
    public class RoleModel {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
    }

    public class UserRolesModel {
        [Required]
        public IList<string> Roles { get; set; }
    }
}