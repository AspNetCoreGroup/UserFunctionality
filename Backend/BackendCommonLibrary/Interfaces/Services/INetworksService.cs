using BackendModelLibrary.Model;

namespace BackendCommonLibrary.Interfaces.Services
{
    public interface INetworksService
    {
        public Task<NetworkDto> GetNetworkAsync(int networkID);

        public Task<IEnumerable<NetworkDto>> GetNetworksAsync();

        public Task CreateNetworkAsync(NetworkDto network);

        public Task UpdateNetworkAsync(int networkID, NetworkDto network);

        public Task DeleteNetworkAsync(int networkID);
    }
}