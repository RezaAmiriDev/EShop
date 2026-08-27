using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EShope.Areas.Identity.Pages.Account
{
    public class LogoutModel : PageModel
    {
        public async Task<IActionResult> OnPostAsync(string returnUrl = "/")
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // حذف کوکی توکن 
            Response.Cookies.Delete("X-Access-Token");

            if(string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
            {
                return RedirectToPage("/Home/HomePage");
            }
            else
            {
                return LocalRedirect(returnUrl);
            }
        }

        public IActionResult OnGet(string returnUrl = "/")
        {
            if (!User.Identity.IsAuthenticated)
            {
                return LocalRedirect(returnUrl);
            }
            return Page();
        }
    }
}
