using ModelLibrary.Model;

namespace BackendCommonLibrary.Interfaces.Services
{
    public interface INetworkUsersService
    {
        public Task<int> FindNetworkUserIDAsync(int networkID, int userID);

        public Task<NetworkUserDto> GetNetworkUserAsync(int requestingUserID, int networkUserID);

        public Task<IEnumerable<NetworkUserDto>> GetNetworkUsersAsync(int requestingUserID);

        public Task<IEnumerable<NetworkUserDto>> GetNetworkUsersAsync(int requestingUserID, int networkID);

        public Task CreateNetworkUserAsync(int requestingUserID, NetworkUserDto networkUser);

        public Task UpdateNetworkUserAsync(int requestingUserID, int networkUserID, NetworkUserDto networkUser);

        public Task DeleteNetworkUserAsync(int requestingUserID, int networkUserID);
    }
}