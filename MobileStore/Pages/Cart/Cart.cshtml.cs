using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ModelLayer.ViewModel;
using ServiceLayer.Services;
using System.Security.Claims;
using System.Text.Json;

namespace EShope.Pages.Cart
{
    public class CartModel : PageModel
    {

        private readonly ILogger<CartModel> _logger;
        private readonly IHttpClientFactory _httpClient;
        private readonly SettingWeb _setting;

        public CartModel(ILogger<CartModel> logger, IHttpClientFactory clientFactory, SettingWeb setting)
        {
            _logger = logger;
            _httpClient = clientFactory;
            _setting = setting;
        }

        public CartDto Cart { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(CancellationToken ct)
        {
            var client = _httpClient.CreateClient(_setting.ClinetName);
            var userId = GetRequestUserId();

            var option = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };


            var resp = await client.GetAsync($"api/cart?userId={Uri.EscapeUriString(userId)}", ct);
            if (!resp.IsSuccessStatusCode)
            {
                _logger.LogWarning("Cart API returned {StatusCode} on GET", resp.StatusCode);
                return NotFound();
            }

            Cart = await resp.Content.ReadFromJsonAsync<CartDto>(option,ct) ?? new CartDto();
            return Page();

        }

        public async Task<IActionResult> OnPostAddAsync(Guid productId, int count, CancellationToken ct)
        {
            var client = _httpClient.CreateClient(_setting.ClinetName);
            var userId = GetRequestUserId();

            var resp = await client.PostAsJsonAsync("api/cart", new
            {
                CustomerId = userId,
                ProductId = productId,
                count = count <= 0 ? 1 :count
            }, ct);

            if (!resp.IsSuccessStatusCode)
            {
                _logger.LogWarning("Cart API returned {StatusCode} on Add", resp.StatusCode);
                TempData["Erorr"] = "محصول به سبد اضافه نشد.";
                return RedirectToPage("/Home/HomePage");
            }

            TempData["Success"] = "محصول به سبد اضافه شد.";
            return RedirectToPage("/Home/HomePage");
        }

        public async Task<IActionResult> OnPostUpdateAsync(Guid productId, int count,CancellationToken ct)
        {
            var client = _httpClient.CreateClient(_setting.ClinetName);
            var userId = GetRequestUserId();

            var response = await client.PutAsJsonAsync($"api/cart/{Uri.EscapeDataString(userId)}/{productId}", new {Count = count}, ct);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Cart API returned {StatusCode} on Update", response.StatusCode);
                TempData["Erorr"] = "بروزرسانی سبد خرید انجام نشد.";
            }

            return RedirectToPage("/Cart/Cart");
        }

        public async Task<IActionResult> OnPostRemoveAsync(Guid productId, CancellationToken ct)
        {

            var client = _httpClient.CreateClient(_setting.ClinetName);
            var userId = GetRequestUserId();

            var response = await client.DeleteAsync($"api/cart/{Uri.EscapeDataString(userId)}/{productId}", ct);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Cart API returned {StatusCode} on Remove", response.StatusCode);
                TempData["Error"] = "حذف محصول از سبد انجام نشد.";
            }
            return RedirectToPage("/Cart/Cart");
        }

        public async Task<IActionResult> OnPostClearAsync(CancellationToken ct)
        {
            var client = _httpClient.CreateClient(_setting.ClinetName);
            var userId = GetRequestUserId();

            var response = await client.DeleteAsync($"api/cart/clear?userId={Uri.EscapeDataString(userId)}", ct);
            if(!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Cart API returned {StatusCode} on Clear", response.StatusCode);
                TempData["Error"] = "خالی‌کردن سبد خرید انجام نشد.";
            }

            return RedirectToPage("/Cart/Cart");
        }

        private string GetRequestUserId()
        {
            if (User?.Identity?.IsAuthenticated == true)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!string.IsNullOrEmpty(userId)) return userId;
            }

            // برای کاربران مهمان، از کوکی یا localStorage استفاده کن
            // اینجا باید یک کوکی یا پارامتر query داشته باشی
            if (Request.Cookies.TryGetValue("GuestCustomerId", out var guestId) && Guid.TryParse(guestId, out _))
                return guestId;

            var newId = Guid.NewGuid();
            Response.Cookies.Append("GuestCustomerId", newId.ToString(), new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(30),
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });
            return newId.ToString();
        }

    }
}
