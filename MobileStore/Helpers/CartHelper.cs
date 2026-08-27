using System.Security.Claims;

namespace EShope.Helpers
{
    public static class CartIdentityHelper
    {
        private const string GuestCookieName = "GuestCustomerId";

        public static string GetOrCreateCustomerId(HttpContext httpContext, ClaimsPrincipal? user)
        {
            if (user?.Identity?.IsAuthenticated == true)
            {
                var claimId = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!string.IsNullOrEmpty(claimId))
                    return claimId;
            }

            if (httpContext.Request.Cookies.TryGetValue(GuestCookieName, out var guestId)
                && Guid.TryParse(guestId, out _))
            {
                return guestId;
            }

            var newId = Guid.NewGuid().ToString();
            httpContext.Response.Cookies.Append(GuestCookieName, newId, new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(30),
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });
            return newId;
        }
    }
}