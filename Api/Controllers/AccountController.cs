using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ModelLayer.ViewModel;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using ModelLayer.Models;
using ClassLibrary;
using ServiceLayer.Services;
using ClassLibrary.Repository;

namespace Api.Controllers.Account
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _config; 
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICustomerRepository _customer;

        public AccountController (SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IConfiguration config, ICustomerRepository customer)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _config = config;
            _customer = customer;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null) return Unauthorized(new { success = false, message = "Invalid credentials." });

            if(await _userManager.IsLockedOutAsync(user))
            {
                var lockoutEnd = await _userManager.GetLockoutEndDateAsync(user);
                var remainingMinutes = lockoutEnd.HasValue ? Math.Max(0, (lockoutEnd.Value - DateTimeOffset.UtcNow).TotalMinutes) : 0;

                return Unauthorized(new
                {
                    success = false,
                    message = $"حساب کاربری شما به دلیل تلاش‌های ناموفق قفل شده است. {Math.Ceiling(remainingMinutes)} دقیقه دیگر مجدداً تلاش کنید.",
                    isLockOut = true,
                    lockoutEnd = lockoutEnd?.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }

           var result = await _signInManager.CheckPasswordSignInAsync(user , model.Password , lockoutOnFailure: true);
            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    var lockoutEnd = await _userManager.GetLockoutEndDateAsync(user);
                    var remainingMinutes = lockoutEnd.HasValue ? Math.Max(0, (lockoutEnd.Value - DateTimeOffset.UtcNow).TotalMinutes) : 0;

                    return Unauthorized(new
                    {
                        success = false,
                        message = $"حساب کاربری شما به دلیل تلاش‌های ناموفق قفل شده است. {Math.Ceiling(remainingMinutes)} دقیقه دیگر مجدداً تلاش کنید.",
                        isLockOut = true,
                        lockoutEnd = lockoutEnd?.ToString("yyyy-MM-dd HH:mm:ss")
                    });
                }
                return Unauthorized(new { success = false, message = "Invalid credentials." });
            }

            // Generate JWT
            var jwtSettings = _config.GetSection("Jwt");
            var keyString = jwtSettings["Key"];
            if (string.IsNullOrEmpty(keyString))
                throw new Exception("JWT Key is not configured.");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var creds = new SigningCredentials(key , SecurityAlgorithms.HmacSha256);

            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name , user.UserName!),
                new Claim(ClaimTypes.NameIdentifier , user.Id)
            };

            foreach(var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpireMinutes"])),
                signingCredentials: creds);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return Ok(new {
                success = true ,
                token = tokenString , 
                username = user.UserName ,
                expires = token.ValidTo
            });
        }


        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterViewModel model) 
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = new ApplicationUser{ UserName = model.Username, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);

            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Name = model.Username,
                Family = "",
                Birth = DateTime.Now,
                NationalCode = "",
                CreateDate = DateTime.UtcNow,
                AddressId = null,
            };

            await _customer.AddAsync(customer, CancellationToken.None);

            // Roles ‌
            //await _userManager.AddToRoleAsync(user, "Cilent");

            // optionally add roles
            return Ok(new { success = true });
        }

        // log out section
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

    }
}
