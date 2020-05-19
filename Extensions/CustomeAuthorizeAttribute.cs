using Microsoft.AspNetCore.Authorization;

namespace IdentityMvc.Extensions
{
    public class CustomeAuthorizeAttribute : AuthorizeAttribute
    {
        private string _roles;
        public CustomeAuthorizeAttribute()
        {
            // Roles = 
        }
    }
}