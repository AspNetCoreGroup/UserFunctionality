using ModelLibrary.Model;

namespace BackendCommonLibrary.Interfaces.Services
{
    public interface IUsersService
    {
        public Task<UserDto> GetUserAsync(int userID);

        public Task<IEnumerable<UserDto>> GetUsersAsync();

        public Task CreateUserAsync(UserDto user);

        public Task UpdateUserAsync(int userID, UserDto user);

        public Task DeleteUserAsync(int userID);
    }
}