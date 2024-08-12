using BackendCommonLibrary.Interfaces.Services;
using BackendService.DataSources;
using BackendService.Model.Entities;
using CommonLibrary.Extensions;
using CommonLibrary.Interfaces.Senders;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.Messages;
using ModelLibrary.Model;
using ModelLibrary.Model.Enums;

namespace BackendService.Services
{
    public class NetworksService : INetworksService
    {
        #region Инициализация

        private ILogger Logger { get; set; }

        private IMessageSender MessageSender { get; set; }

        private BackendContext Context { get; set; }


        public NetworksService(ILoggerFactory loggerFactory, IMessageSender messageSender, BackendContext context)
        {
            Logger = loggerFactory.CreateLogger<NetworksService>();
            MessageSender = messageSender;
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

            Context.Add(network);

            await Context.SaveChangesAsync();

            var networkUser = new NetworkUser()
            {
                NetworkID = network.NetworkID,
                UserID = requestingUserID,
                IsAdmin = true,
                IsEditor = true
            };

            Context.Add(networkUser);

            await Context.SaveChangesAsync();

            await NotifyDataEventAsync(network, DataEventOperationType.Add);
            await NotifyDataEventAsync(networkUser, DataEventOperationType.Add);
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

            await NotifyDataEventAsync(network, DataEventOperationType.Update);
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

            await NotifyDataEventAsync(network, DataEventOperationType.Delete);
        }

        #endregion

        #region Вспомогательное

        private static NetworkDto Convert(Network network)
        {
            return new NetworkDto()
            {
                NetworkID = network.NetworkID,
                NetworkTitle = network.NetworkTitle
            };
        }

        private async Task NotifyDataEventAsync(Network network, DataEventOperationType operationType)
        {
            try
            {
                var dataEvent = new DataEventMessage<NetworkDto>()
                {
                    Operation = operationType,
                    Data = Convert(network)
                };

                await MessageSender.SendMessageAsync("BackendAll", dataEvent);
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, "Error on user event notification. Data may be lost.");
            }
        }

        // TODO: Подумать, как быть с этой ситуацией. Неправильно, что код дублируется.
        private static NetworkUserDto Convert(NetworkUser networkUser)
        {
            return new NetworkUserDto()
            {
                NetworkUserID = networkUser.NetworkUserID,
                NetworkID = networkUser.NetworkID,
                UserID = networkUser.UserID,
                IsAdmin = networkUser.IsAdmin,
                IsEditor = networkUser.IsEditor
            };
        }

        private async Task NotifyDataEventAsync(NetworkUser user, DataEventOperationType operationType)
        {
            try
            {
                var dataEvent = new DataEventMessage<NetworkUserDto>()
                {
                    Operation = operationType,
                    Data = Convert(user)
                };

                await MessageSender.SendMessageAsync("BackendAll", dataEvent);
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, "Error on user event notification. Data may be lost.");
            }
        }

        #endregion
    }
}