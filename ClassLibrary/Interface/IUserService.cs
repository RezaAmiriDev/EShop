using ClassLibrary.Repository;
using EShope.ViewModels;
using ModelLayer.ViewModel;


namespace ModelLayer.Interface
{
    public interface IUserService
    {
        Task<List<UserDto>> GetAllUsersAsync();
        Task<bool> CreateUserAsync(UserDto dto);
        Task<bool> UpdateUserAsync(string id, UserDto dto);
        Task<bool> DeleteUserAsync(string id);
    }
}

