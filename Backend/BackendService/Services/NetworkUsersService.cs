using BackendCommonLibrary.Interfaces.Services;
using BackendService.DataSources;
using BackendService.Model.Entities;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.Model;
using System.Linq.Expressions;

namespace BackendService.Services
{
    public class NetworkUsersService : INetworkUsersService
    {
        #region Инициализация

        private ILogger Logger { get; set; }

        private BackendContext Context { get; set; }


        public NetworkUsersService(ILoggerFactory loggerFactory, BackendContext context)
        {
            Logger = loggerFactory.CreateLogger<NetworksService>();
            Context = context;
        }

        #endregion

        #region Функционал

        public async Task<int> FindNetworkUserIDAsync(int networkID, int userID)
        {
            var networkUser = await Context.NetworkUsers.FirstOrDefaultAsync(x => x.NetworkID == networkID && x.UserID == userID)
                ?? throw new Exception("Пользователь не обнаружен в данной сети.");

            return networkUser.NetworkUserID;
        }

        public async Task<NetworkUserDto> GetNetworkUserAsync(int requestingUserID, int networkUserID)
        {
            var networksQuery = GetUserNetworks(requestingUserID);

            var usersQuery = Context.NetworkUsers.Join(networksQuery,
                (u) => u.NetworkID,
                (n) => n.NetworkID,
                (d, n) => d);

            var user = await usersQuery.FirstOrDefaultAsync(x => x.NetworkUserID == networkUserID)
                ?? throw new Exception("Пользователь в сети не существует, или у вас нет к нему доступа.");

            return Convert(user);
        }

        public async Task<IEnumerable<NetworkUserDto>> GetNetworkUsersAsync(int requestingUserID)
        {
            var networksQuery = GetUserNetworks(requestingUserID);

            var usersQuery = Context.NetworkUsers.Join(networksQuery,
                (u) => u.NetworkID,
                (n) => n.NetworkID,
                (d, n) => d);

            var users = await usersQuery.ToListAsync();

            return users.Select(Convert).ToList();
        }


        public async Task<IEnumerable<NetworkUserDto>> GetNetworkUsersAsync(int requestingUserID, int networkID)
        {
            var networksQuery = GetUserNetworks(requestingUserID).Where(x => x.NetworkID == networkID);

            var usersQuery = Context.NetworkUsers.Join(networksQuery,
                (u) => u.NetworkID,
                (n) => n.NetworkID,
                (d, n) => d);

            var users = await usersQuery.ToListAsync();

            return users.Select(Convert).ToList();
        }

        public async Task CreateNetworkUserAsync(int requestingUserID, NetworkUserDto networkUserDto)
        {
            if (!await CheckUserCanEditAsync(requestingUserID, networkUserDto.NetworkID))
            {
                throw new Exception("У вас нет доступу к добавлению пользователей в данной сети.");
            }

            var networkUser = new NetworkUser()
            {
                NetworkID = networkUserDto.NetworkID,
                UserID = networkUserDto.UserID,
            };

            Context.Add(networkUser);

            await Context.SaveChangesAsync();
        }

        public async Task UpdateNetworkUserAsync(int requestingUserID, int networkUserID, NetworkUserDto networkUserDto)
        {
            var networkUser = await GetNetworkUserAsync(networkUserID) ?? throw new Exception("Пользователь в сети не существует");

            if (!await CheckUserCanEditAsync(requestingUserID, networkUser.NetworkID))
            {
                throw new Exception("У вас нет доступу к редактированию пользователей в данной сети.");
            }

            networkUser.IsAdmin = networkUserDto.IsAdmin;
            networkUser.IsEditor = networkUserDto.IsEditor;

            await Context.SaveChangesAsync();
        }

        public async Task DeleteNetworkUserAsync(int requestingUserID, int networkUserID)
        {
            var networkUser = await GetNetworkUserAsync(networkUserID) ?? throw new Exception("Пользователь в сети не существует");

            if (!await CheckUserCanEditAsync(requestingUserID, networkUser.NetworkID))
            {
                throw new Exception("У вас нет доступа к удалению устройств в данной сети.");
            }

            Context.Remove(networkUser);

            await Context.SaveChangesAsync();
        }

        #endregion

        #region Вспомогательное

        private IQueryable<Network> GetUserNetworks(int userID)
        {
            var activeNetworks = Context.Networks.Where(x => x.IsDeleted);
            var userNetworks = Context.NetworkUsers.Where(x => x.UserID == userID);

            var networksQuery = activeNetworks.Join(userNetworks,
                (n) => n.NetworkID,
                (u) => u.NetworkID,
                (n, u) => n);

            return networksQuery;
        }

        private IQueryable<Network> GetUserNetworks(int userID, Expression<Func<NetworkUser, bool>> filter)
        {
            var activeNetworks = Context.Networks.Where(x => x.IsDeleted);
            var userNetworks = Context.NetworkUsers.Where(x => x.UserID == userID).Where(filter);

            var networksQuery = activeNetworks.Join(userNetworks,
                (n) => n.NetworkID,
                (u) => u.NetworkID,
                (n, u) => n);

            return networksQuery;
        }

        private Task<NetworkUser?> GetNetworkUserAsync(int networkUserID)
        {
            return Context.NetworkUsers.FirstOrDefaultAsync(x => x.NetworkUserID == networkUserID);
        }

        private Task<bool> CheckUserCanEditAsync(int userID, int networkID)
        {
            var networksQuery = GetUserNetworks(userID, x => x.IsAdmin);

            return networksQuery.AnyAsync(x => x.NetworkID == networkID);
        }

        private static NetworkUserDto Convert(NetworkUser networkUser)
        {
            return new NetworkUserDto()
            {
                NetworkUserID = networkUser.NetworkUserID,
                NetworkID = networkUser.NetworkID,
                UserID = networkUser.UserID
            };
        }

        #endregion
    }
}