using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityMvc.Areas.Identity.Pages.Account
{
    public class AccessDeniedModel : PageModel
    {
        public void OnGet(string returnUrl){
            ViewData["ReturnUrl"] = returnUrl;
        }
    }
}