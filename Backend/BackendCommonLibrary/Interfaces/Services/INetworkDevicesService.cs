using BackendModelLibrary.Model;

namespace BackendCommonLibrary.Interfaces.Services
{
    public interface INetworkDevicesService
    {
        public Task<NetworkDeviceDto> GetNetworkDeviceAsync(int networkDeviceID);

        public Task<IEnumerable<NetworkDeviceDto>> GetNetworkDevicesAsync();

        public Task CreateNetworkDeviceAsync(NetworkDeviceDto networkDevice);

        public Task UpdateNetworkDeviceAsync(int networkDeviceID, NetworkDeviceDto networkDevice);

        public Task DeleteNetworkDeviceAsync(int networkDeviceID);
    }
}