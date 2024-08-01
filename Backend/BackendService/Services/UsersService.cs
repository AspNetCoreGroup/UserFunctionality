using BackendService.DataSources;
using BackendService.Model.Entities;
using CommonLibrary.Extensions;
using BackendCommonLibrary.Interfaces.Services;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.Model;

namespace BackendService.Services
{
    public class UsersService : IUsersService
    {
        #region Поля

        private ILogger Logger { get; }

        private BackendContext Context { get; }

        #endregion

        #region Функционал

        public UsersService(ILoggerFactory loggerFactory, BackendContext context)
        {
            Logger = loggerFactory.CreateLogger<UsersService>();
            Context = context;
        }

        public async Task<UserDto> GetUserAsync(int userID)
        {
            var user = await Context.Users.FindAsync(userID) ?? throw new KeyNotFoundException($"User with userID {userID}");

            return Convert(user);
        }

        public async Task<IEnumerable<UserDto>> GetUsersAsync()
        {
            var users = await Context.Users.ToListAsync();

            return users.Select(Convert);
        }

        public async Task CreateUserAsync(UserDto userDto)
        {
            var user = Convert(userDto);

            Context.Add(user);
            await Context.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(int userID, UserDto userDto)
        {
            var user = Convert(userDto);

            user.UserID = userID;

            Context.Attach(user);
            await Context.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(int userID)
        {
            var user = new User()
            {
                UserID = userID,
                UserLogin = "",
                UserPassword = "",
                FirstName = "",
                LastName = "",
                Patronymic = null
            };

            Context.Remove(user);

            await Context.SaveChangesAsync();
        }

        private static UserDto Convert(User user)
        {
            return new UserDto()
            {
                UserID = user.UserID,
                UserLogin = user.UserLogin,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Patronymic = user.Patronymic,
                Email = user.Email,
                NotificationEmail = user.NotificationEmail,
                NotificationTelegramID = user.NotificationTelegramID,
            };
        }

        private static User Convert(UserDto user)
        {
            return new User()
            {
                UserID = user.UserID,
                UserLogin = user.UserLogin,
                UserPassword = "",
                FirstName = user.FirstName,
                LastName = user.LastName,
                Patronymic = user.Patronymic,
                Email = user.Email,
                NotificationEmail = user.NotificationEmail,
                NotificationTelegramID = user.NotificationTelegramID,
            };
        }

        #endregion
    }
}