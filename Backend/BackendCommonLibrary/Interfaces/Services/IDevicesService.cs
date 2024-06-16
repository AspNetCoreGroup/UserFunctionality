using BackendModelLibrary.Model;

namespace BackendCommonLibrary.Interfaces.Services
{
    public interface IDevicesService
    {
        public Task<DeviceDto> GetDeviceAsync(int deviceID);

        public Task<IEnumerable<DeviceDto>> GetDevicesAsync();

        public Task CreateDeviceAsync(DeviceDto device);

        public Task UpdateDeviceAsync(int deviceID, DeviceDto device);

        public Task DeleteDeviceAsync(int deviceID);
    }
}