using BackendCommonLibrary.Interfaces.Services;
using BackendService.DataSources;
using BackendService.Model.Entities;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.Model;

namespace BackendService.Services
{
    public class NetworksService : INetworksService
    {
        #region Инициализация

        private ILogger Logger { get; set; }

        private BackendContext Context { get; set; }


        public NetworksService(ILoggerFactory loggerFactory, BackendContext context)
        {
            Logger = loggerFactory.CreateLogger<NetworksService>();
            Context = context;
        }

        #endregion

        #region Функционал

        public async Task<NetworkDto> GetNetworkAsync(int requestingUserID, int networkID)
        {
            var activeNetworks = Context.Networks.Where(x => !x.IsDeleted);
            var userNetworks = Context.NetworkUsers.Where(x => x.UserID == requestingUserID);

            var networksQuery = activeNetworks.Join(userNetworks,
                (n) => n.NetworkID,
                (u) => u.NetworkID,
                (n, u) => n);

            var network = await networksQuery.FirstOrDefaultAsync(x => x.NetworkID == networkID)
                ?? throw new Exception("Cannot find the network because it does not exist or you do not have permissions");

            return Convert(network);
        }

        public async Task<IEnumerable<NetworkDto>> GetNetworksAsync(int requestingUserID)
        {
            var activeNetworks = Context.Networks.Where(x => !x.IsDeleted);
            var userNetworks = Context.NetworkUsers.Where(x => x.UserID == requestingUserID);

            var networksQuery = activeNetworks.Join(userNetworks,
                (n) => n.NetworkID,
                (u) => u.NetworkID,
                (n, u) => n);

            var networks = await networksQuery.ToListAsync();

            return networks.Select(Convert).ToList();
        }

        public async Task CreateNetworkAsync(int requestingUserID, NetworkDto networkDto)
        {
            var network = new Network()
            {
                CreatorUserID = requestingUserID,
                NetworkTitle = networkDto.NetworkTitle
            };

            var networkUser = new NetworkUser()
            {
                Network = network,
                UserID = requestingUserID,
                IsAdmin = true,
                IsEditor = true
            };

            Context.Add(network);
            Context.Add(networkUser);

            await Context.SaveChangesAsync();

            Logger.LogInformation("1231212312");
        }

        public async Task UpdateNetworkAsync(int requestingUserID, int networkID, NetworkDto networkDto)
        {
            var activeNetworks = Context.Networks.Where(x => !x.IsDeleted);
            var userNetworks = Context.NetworkUsers.Where(x => x.UserID == requestingUserID && x.IsAdmin);

            var networksQuery = activeNetworks.Join(userNetworks,
                (n) => n.NetworkID,
                (u) => u.NetworkID,
                (n, u) => n);

            var network = await networksQuery.FirstOrDefaultAsync(x => x.NetworkID == networkID)
                ?? throw new Exception("Cannot find the network because it does not exist or you do not have permissions");

            network.NetworkTitle = networkDto.NetworkTitle;

            await Context.SaveChangesAsync();
        }

        public async Task DeleteNetworkAsync(int requestingUserID, int networkID)
        {
            var activeNetworks = Context.Networks.Where(x => !x.IsDeleted);
            var userNetworks = Context.NetworkUsers.Where(x => x.UserID == requestingUserID && x.IsAdmin);

            var networksQuery = activeNetworks.Join(userNetworks,
                (n) => n.NetworkID,
                (u) => u.NetworkID,
                (n, u) => n);

            var network = await networksQuery.FirstOrDefaultAsync(x => x.NetworkID == networkID)
                ?? throw new Exception("Cannot find the network because it does not exist or you do not have permissions");

            network.IsDeleted = true;

            await Context.SaveChangesAsync();
        }

        private static NetworkDto Convert(Network network)
        {
            return new NetworkDto()
            {
                NetworkID = network.NetworkID,
                NetworkTitle = network.NetworkTitle
            };
        }

        #endregion
    }
}