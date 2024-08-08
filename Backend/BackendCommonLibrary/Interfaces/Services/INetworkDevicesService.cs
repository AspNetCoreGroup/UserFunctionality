using ModelLibrary.Model;

namespace BackendCommonLibrary.Interfaces.Services
{
    public interface INetworkDevicesService
    {
        public Task<int> FindNetworkDeviceIDAsync(int networkID, int deviceID);

        public Task<NetworkDeviceDto> GetNetworkDeviceAsync(int requestingUserID, int networkDeviceID);

        public Task<IEnumerable<NetworkDeviceDto>> GetNetworkDevicesAsync(int requestingUserID);

        public Task<IEnumerable<NetworkDeviceDto>> GetNetworkDevicesAsync(int requestingUserID, int networkID);

        public Task CreateNetworkDeviceAsync(int requestingUserID, NetworkDeviceDto networkDevice);

        public Task UpdateNetworkDeviceAsync(int requestingUserID, int networkDeviceID, NetworkDeviceDto networkDevice);

        public Task DeleteNetworkDeviceAsync(int requestingUserID, int networkDeviceID);
    }
}