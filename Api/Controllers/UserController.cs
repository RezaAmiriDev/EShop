using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Interface;
using ModelLayer.ViewModel;


namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "admin")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserDto dto)
        {
            if (string.IsNullOrEmpty(dto.Password))
                return BadRequest(new { message = "رمز عبور اجباری است" });

            var success = await _userService.CreateUserAsync(dto);
            if (!success)
                return BadRequest(new { message = "خطا در ایجاد کاربر" });

            return Ok(new { success = true });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, UserDto dto)
        {
            var success = await _userService.UpdateUserAsync(id, dto);
            if (!success)
                return NotFound(new { message = "کاربر یافت نشد" });

            return Ok(new { success = true });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var success = await _userService.DeleteUserAsync(id);
            if (!success)
                return NotFound(new { message = "کاربر یافت نشد" });

            return Ok(new { success = true });
        }
    }
}