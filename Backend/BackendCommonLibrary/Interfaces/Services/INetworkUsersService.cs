using BackendModelLibrary.Model;

namespace BackendCommonLibrary.Interfaces.Services
{
    public interface INetworkUsersService
    {
        public Task<NetworkUserDto> GetNetworkUserAsync(int networkUserID);

        public Task<IEnumerable<NetworkUserDto>> GetNetworkUsersAsync();

        public Task CreateNetworkUserAsync(NetworkUserDto networkUser);

        public Task UpdateNetworkUserAsync(int networkUserID, NetworkUserDto networkUser);

        public Task DeleteNetworkUserAsync(int networkUserID);
    }
}