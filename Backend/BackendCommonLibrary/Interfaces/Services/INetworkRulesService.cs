using ModelLibrary.Model;

namespace BackendCommonLibrary.Interfaces.Services
{
    public interface INetworkRulesService
    {
        public Task<NetworkRuleDto> GetNetworkRuleAsync(int requestingUserID, int networkRuleID);

        public Task<IEnumerable<NetworkRuleDto>> GetNetworkRulesAsync(int requestingUserID);

        public Task<IEnumerable<NetworkRuleDto>> GetNetworkRulesAsync(int requestingUserID, int networkID);

        public Task CreateNetworkRuleAsync(int requestingUserID, NetworkRuleDto networkRule);

        public Task UpdateNetworkRuleAsync(int requestingUserID, int networkRuleID, NetworkRuleDto networkRule);

        public Task DeleteNetworkRuleAsync(int requestingUserID, int networkRuleID);
    }
}