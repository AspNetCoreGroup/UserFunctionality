using BackendCommonLibrary.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using ModelLibrary.Model;

namespace Backend.Controllers;

[ApiController]
public class NetworkRulesController : ControllerBase
{
    private ILogger Logger { get; }

    private INetworkRulesService NetworkRulesService { get; set; }


    public NetworkRulesController(ILoggerFactory loggerFactory, INetworkRulesService networkRulesService)
    {
        Logger = loggerFactory.CreateLogger<NetworkRulesController>();
        NetworkRulesService = networkRulesService;
    }

    #region Прямой вызов

    [HttpGet("NetworkRules")]
    public async Task<IActionResult> GetAsync(int requestingUserID)
    {
        var networkRule = await NetworkRulesService.GetNetworkRulesAsync(requestingUserID);

        return Ok(networkRule);
    }

    [HttpGet("NetworkRules/{networkRuleID}")]
    public async Task<IActionResult> GetAsync(int requestingUserID, int networkRuleID)
    {
        var networkRules = await NetworkRulesService.GetNetworkRuleAsync(requestingUserID, networkRuleID);

        return Ok(networkRules);
    }

    [HttpPost("NetworkRules")]
    public async Task<IActionResult> AddAsync(int requestingUserID, NetworkRuleDto networkRule)
    {
        await NetworkRulesService.CreateNetworkRuleAsync(requestingUserID, networkRule);

        return Ok();
    }

    [HttpPut("NetworkRules/{networkRuleID}")]
    public async Task<IActionResult> UpdateAsync(int requestingUserID, int networkRuleID, NetworkRuleDto networkRule)
    {
        await NetworkRulesService.UpdateNetworkRuleAsync(requestingUserID, networkRuleID, networkRule);

        return Ok();
    }

    [HttpDelete("NetworkRules/{networkRuleID}")]
    public async Task<IActionResult> DeleteAsync(int requestingUserID, int networkRuleID)
    {
        await NetworkRulesService.DeleteNetworkRuleAsync(requestingUserID, networkRuleID);

        return Ok();
    }

    #endregion

    #region Вызов через сеть

    [HttpGet("Network/{networkID}/NetworkRules/NetworkRules/")]
    public async Task<IActionResult> Get2Async(int requestingUserID, int networkID)
    {
        var networkRules = await NetworkRulesService.GetNetworkRulesAsync(requestingUserID, networkID);

        return Ok(networkRules);
    }

    #endregion

    #region Вспомогательное

    private void ValidateNetworkID(NetworkRuleDto networkRule, int networkID)
    {
        if (networkRule.NetworkID != networkID)
        {
            throw new Exception("Свойство NetworkRule.NetworkID не совпадает с параметрами запроса.");
        }
    }

    private void ValidateUserID(NetworkRuleDto networkRule, int userID)
    {
        if (networkRule.UserID != userID)
        {
            throw new Exception("Свойство NetworkRule.RuleID не совпадает с параметрами запроса.");
        }
    }

    #endregion
}