using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DataLayer.Hellper
{
    public class AddBearerTokenHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AddBearerTokenHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var ctx = _httpContextAccessor.HttpContext;
            if (ctx != null)
            {
                // سعی کن اول توکن را از Claim بخوانی (اگر در ساین-این local ذخیره کردی)
                var token = ctx.User?.FindFirst("access_token")?.Value;

                // اگر در claim نبود، از کوکی بخوان
                if (string.IsNullOrEmpty(token))
                {
                    token = ctx.Request?.Cookies["X-Access-Token"];
                }

                if (!string.IsNullOrEmpty(token))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}

