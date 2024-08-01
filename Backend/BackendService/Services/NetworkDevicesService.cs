using BackendCommonLibrary.Interfaces.Services;
using BackendService.DataSources;
using BackendService.Model.Entities;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.Model;
using System.Linq.Expressions;

namespace BackendService.Services
{
    public class NetworkDevicesService : INetworkDevicesService
    {
        #region Инициализация

        private ILogger Logger { get; set; }

        private BackendContext Context { get; set; }


        public NetworkDevicesService(ILoggerFactory loggerFactory, BackendContext context)
        {
            Logger = loggerFactory.CreateLogger<NetworksService>();
            Context = context;
        }

        #endregion

        #region Функционал

        public async Task<int> FindNetworkDeviceIDAsync(int networkID, int deviceID)
        {
            var networkDevice = await Context.NetworkDevices.FirstOrDefaultAsync(x => x.NetworkID == networkID && x.DeviceID == deviceID)
                ?? throw new Exception("Устройство не обнаружено в данной сети.");

            return networkDevice.NetworkDeviceID;
        }

        public async Task<NetworkDeviceDto> GetNetworkDeviceAsync(int requestingUserID, int networkDeviceID)
        {
            var networksQuery = GetUserNetworks(requestingUserID);

            var devicesQuery = Context.NetworkDevices.Join(networksQuery,
                (d) => d.NetworkID,
                (n) => n.NetworkID,
                (d, n) => d);

            var device = await devicesQuery.FirstOrDefaultAsync(x => x.NetworkDeviceID == networkDeviceID)
                ?? throw new Exception("Устройство в сети не существует, или у вас нет к нему доступа.");

            return Convert(device);
        }

        public async Task<IEnumerable<NetworkDeviceDto>> GetNetworkDevicesAsync(int requestingUserID)
        {
            var networksQuery = GetUserNetworks(requestingUserID);

            var devicesQuery = Context.NetworkDevices.Join(networksQuery,
                (d) => d.NetworkID,
                (n) => n.NetworkID,
                (d, n) => d);

            var devices = await devicesQuery.ToListAsync();

            return devices.Select(Convert).ToList();
        }

        public async Task<IEnumerable<NetworkDeviceDto>> GetNetworkDevicesAsync(int requestingUserID, int networkID)
        {
            var networksQuery = GetUserNetworks(requestingUserID).Where(x => x.NetworkID == networkID);

            var devicesQuery = Context.NetworkDevices.Join(networksQuery,
                (d) => d.NetworkID,
                (n) => n.NetworkID,
                (d, n) => d);

            var devices = await devicesQuery.ToListAsync();

            return devices.Select(Convert).ToList();
        }

        public async Task CreateNetworkDeviceAsync(int requestingUserID, NetworkDeviceDto networkDeviceDto)
        {
            if (!await CheckUserCanEditAsync(requestingUserID, networkDeviceDto.NetworkID))
            {
                throw new Exception("У вас нет доступу к добавлению устройств в данной сети.");
            }

            var networkDevice = new NetworkDevice()
            {
                NetworkID = networkDeviceDto.NetworkID,
                DeviceID = networkDeviceDto.DeviceID,
            };

            Context.Add(networkDevice);

            await Context.SaveChangesAsync();
        }

        public async Task UpdateNetworkDeviceAsync(int requestingUserID, int networkDeviceID, NetworkDeviceDto networkDeviceDto)
        {
            var networkDevice = await GetNetworkDeviceAsync(networkDeviceID) ?? throw new Exception("Устройство в сети не существует");

            if (!await CheckUserCanEditAsync(requestingUserID, networkDevice.NetworkID))
            {
                throw new Exception("У вас нет доступу к редактированию устройств в данной сети.");
            }

            throw new Exception("Редактирование устройств в сети на данный момент не реализовано.");

            //await Context.SaveChangesAsync();
        }

        public async Task DeleteNetworkDeviceAsync(int requestingUserID, int networkDeviceID)
        {
            var networkDevice = await GetNetworkDeviceAsync(networkDeviceID) ?? throw new Exception("Устройство в сети не существует");

            if (!await CheckUserCanEditAsync(requestingUserID, networkDevice.NetworkID))
            {
                throw new Exception("У вас нет доступа к удалению устройств в данной сети.");
            }

            Context.Remove(networkDevice);

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

        private Task<NetworkDevice?> GetNetworkDeviceAsync(int networkDeviceID)
        {
            return Context.NetworkDevices.FirstOrDefaultAsync(x => x.NetworkDeviceID == networkDeviceID);
        }

        private Task<bool> CheckUserCanEditAsync(int userID, int networkID)
        {
            var networksQuery = GetUserNetworks(userID, x => x.IsAdmin);

            return networksQuery.AnyAsync(x => x.NetworkID == networkID);
        }

        private static NetworkDeviceDto Convert(NetworkDevice networkDevice)
        {
            return new NetworkDeviceDto()
            {
                NetworkDeviceID = networkDevice.NetworkDeviceID,
                NetworkID = networkDevice.NetworkID,
                DeviceID = networkDevice.DeviceID
            };
        }

        #endregion
    }
}