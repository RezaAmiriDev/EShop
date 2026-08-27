using EShope.ViewModels;
using Microsoft.AspNetCore.Identity;
using ModelLayer.Interface;
using ModelLayer.Models;
using ModelLayer.ViewModel;

namespace ServiceLayer.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public UserService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            this.roleManager = roleManager;
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            var users = _userManager.Users.ToList();
            var result = new List<UserDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                result.Add(new UserDto
                {
                    Id = user.Id,
                    UserName = user.UserName!,
                    Email = user.Email,
                    Role = roles.FirstOrDefault() ?? ""
                });
            }
            return result;
        }

        public async Task<bool> CreateUserAsync(UserDto dto)
        {
            if (string.IsNullOrEmpty(dto.Password)) return false;

            var user = new ApplicationUser { UserName = dto.UserName, Email = dto.Email };
            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded) return false;

            if (!string.IsNullOrEmpty(dto.Role))
            {
                var roleExists = await roleManager.RoleExistsAsync(dto.Role);
                if (!roleExists) return false; // یا لاگ کنید
                var addResult = await _userManager.AddToRoleAsync(user, dto.Role);
                if (!addResult.Succeeded) return false;
            }

            return true;
        }

        public async Task<bool> UpdateUserAsync(string id, UserDto dto)
        {
            var users = await _userManager.FindByIdAsync(id);
            if (users == null) return false;

            users.UserName = dto.UserName;
            users.Email = dto.Email;

            if (!string.IsNullOrEmpty(dto.Password))
            {
                users.PasswordHash = _userManager.PasswordHasher.HashPassword(users, dto.Password);
            }

            var result = await _userManager.UpdateAsync(users);
            if(!result.Succeeded) return false;

            var currentRole = await _userManager.GetRolesAsync(users);
            var removeRole = await _userManager.RemoveFromRolesAsync(users, currentRole);
            if(!removeRole.Succeeded) return false;


            if (!string.IsNullOrEmpty(dto.Role))
            {
                var addRole = await _userManager.AddToRoleAsync(users, dto.Role);
                if (!addRole.Succeeded) return false;
            }

            return true;
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return false;

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }
    }
}
