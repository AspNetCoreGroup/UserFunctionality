using CommonLibrary.Extensions;
using CommonLibrary.Interfaces.Senders;
using CommonLibrary.Interfaces.Services;
using ModelLibrary.Messages;
using ModelLibrary.Model;
using ModelLibrary.Model.Enums;
using BackendService.DataSources;
using BackendService.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace BackendService.Services
{
    public class UsersService : IUsersService
    {
        #region Поля

        private ILogger Logger { get; }

        private IMessageSender MessageSender { get; }

        private BackendContext Context { get; }

        #endregion

        #region Функционал

        public UsersService(ILoggerFactory loggerFactory, IMessageSender messageSender, BackendContext context)
        {
            Logger = loggerFactory.CreateLogger<UsersService>();
            MessageSender = messageSender;
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

            await NotifyUserDataEventAsync(user, DataEventOperationType.Add);
        }

        public async Task UpdateUserAsync(int userID, UserDto userDto)
        {
            var user = Convert(userDto);

            user.UserID = userID;

            Context.Attach(user);
            await Context.SaveChangesAsync();

            await NotifyUserDataEventAsync(user, DataEventOperationType.Update);
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

            await NotifyUserDataEventAsync(user, DataEventOperationType.Delete);
        }

        private async Task NotifyUserDataEventAsync(User user, DataEventOperationType operationType)
        {
            try
            {
                var dataEvent = new DataEventMessage<User>()
                {
                    Operation = operationType,
                    Data = user
                };

                await MessageSender.SendMessageAsync("BackendAll", dataEvent);
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, "Error on user event notification. Data may be lost.");
            }
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