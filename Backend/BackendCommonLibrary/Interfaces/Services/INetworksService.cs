using ModelLibrary.Model;

namespace BackendCommonLibrary.Interfaces.Services
{
    public interface INetworksService
    {
        public Task<NetworkDto> GetNetworkAsync(int requestingUserID, int networkID);

        public Task<IEnumerable<NetworkDto>> GetNetworksAsync(int requestingUserID);

        public Task CreateNetworkAsync(int requestingUserID, NetworkDto network);

        public Task UpdateNetworkAsync(int requestingUserID, int networkID, NetworkDto network);

        public Task DeleteNetworkAsync(int requestingUserID, int networkID);
    }
}