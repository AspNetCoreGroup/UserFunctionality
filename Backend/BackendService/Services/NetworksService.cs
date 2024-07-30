using CommonLibrary.Interfaces.Services;
using ModelLibrary.Model;
using BackendService.DataSources;
using BackendService.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace BackendService.Services
{
    public class NetworksService : INetworksService
    {
        private ILogger Logger { get; set; }

        private BackendContext Context { get; set; }


        public NetworksService(ILoggerFactory loggerFactory, BackendContext context)
        {
            Logger = loggerFactory.CreateLogger<NetworksService>();
            Context = context;
        }

        public async Task<NetworkDto> GetNetworkAsync(int networkID)
        {
            var network = await Context.Networks.FindAsync(networkID) ?? throw new KeyNotFoundException($"Network with networkID {networkID}");

            return Convert(network);
        }

        public async Task<IEnumerable<NetworkDto>> GetNetworksAsync()
        {
            var networks = await Context.Networks.ToListAsync();

            return networks.Select(Convert);
        }

        public async Task CreateNetworkAsync(NetworkDto networkDto)
        {
            var network = Convert(networkDto);

            Context.Add(network);
            await Context.SaveChangesAsync();
        }

        public async Task UpdateNetworkAsync(int networkID, NetworkDto networkDto)
        {
            var network = Convert(networkDto);

            network.NetworkID = networkID;

            Context.Attach(network);
            await Context.SaveChangesAsync();
        }

        public async Task DeleteNetworkAsync(int networkID)
        {
            var network = new Network()
            {
                NetworkID = networkID,
                NetworkTitle = ""
            };

            Context.Remove(network);

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

        private static Network Convert(NetworkDto network)
        {
            return new Network()
            {
                NetworkID = network.NetworkID,
                NetworkTitle = network.NetworkTitle
            };
        }
    }
}