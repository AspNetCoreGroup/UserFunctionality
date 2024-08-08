using BackendCommonLibrary.Interfaces.Services;
using BackendService.DataSources;
using BackendService.Model.Entities;
using CommonLibrary.Extensions;
using CommonLibrary.Interfaces.Senders;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.Messages;
using ModelLibrary.Model;
using ModelLibrary.Model.Enums;
using System.Linq.Expressions;

namespace BackendService.Services
{
    public class NetworkRulesService : INetworkRulesService
    {
        #region Инициализация

        private ILogger Logger { get; set; }

        private IMessageSender MessageSender { get; }

        private BackendContext Context { get; set; }


        public NetworkRulesService(ILoggerFactory loggerFactory, IMessageSender messageSender, BackendContext context)
        {
            Logger = loggerFactory.CreateLogger<NetworksService>();
            MessageSender = messageSender;
            Context = context;
        }

        #endregion

        #region Функционал

        public async Task<NetworkRuleDto> GetNetworkRuleAsync(int requestingUserID, int networkRuleID)
        {
            var networksQuery = GetUserNetworks(requestingUserID);

            var RulesQuery = Context.NetworkRules.Join(networksQuery,
                (d) => d.NetworkID,
                (n) => n.NetworkID,
                (d, n) => d);

            var Rule = await RulesQuery.FirstOrDefaultAsync(x => x.NetworkRuleID == networkRuleID)
                ?? throw new Exception("Правило не существует, или у вас нет к нему доступа.");

            return Convert(Rule);
        }

        public async Task<IEnumerable<NetworkRuleDto>> GetNetworkRulesAsync(int requestingUserID)
        {
            var networksQuery = GetUserNetworks(requestingUserID);

            var RulesQuery = Context.NetworkRules.Join(networksQuery,
                (d) => d.NetworkID,
                (n) => n.NetworkID,
                (d, n) => d);

            var Rules = await RulesQuery.ToListAsync();

            return Rules.Select(Convert).ToList();
        }

        public async Task<IEnumerable<NetworkRuleDto>> GetNetworkRulesAsync(int requestingUserID, int networkID)
        {
            var networksQuery = GetUserNetworks(requestingUserID).Where(x => x.NetworkID == networkID);

            var RulesQuery = Context.NetworkRules.Join(networksQuery,
                (d) => d.NetworkID,
                (n) => n.NetworkID,
                (d, n) => d);

            var Rules = await RulesQuery.ToListAsync();

            return Rules.Select(Convert).ToList();
        }

        public async Task CreateNetworkRuleAsync(int requestingUserID, NetworkRuleDto networkRuleDto)
        {
            if (!await CheckUserCanEditAsync(requestingUserID, networkRuleDto.NetworkID))
            {
                throw new Exception("У вас нет доступу к добавлению правил в данной сети.");
            }

            if (networkRuleDto.RuleExpression == null && networkRuleDto.Rule == null)
            {
                throw new Exception("В правиле должно быть указано строковое правила, либо структурированное правило.");
            }

            var networkRule = new NetworkRule()
            {
                NetworkID = networkRuleDto.NetworkID,
                UserID = networkRuleDto.UserID,
                NotificationType = networkRuleDto.NotificationType,
                RuleExpression = networkRuleDto.RuleExpression ?? ParseRule(networkRuleDto.Rule!),
            };

            Context.Add(networkRule);

            await Context.SaveChangesAsync();

            await NotifyUserDataEventAsync(networkRule!, DataEventOperationType.Add);
        }

        public async Task UpdateNetworkRuleAsync(int requestingUserID, int networkRuleID, NetworkRuleDto networkRuleDto)
        {
            var networkRule = await GetNetworkRuleAsync(networkRuleID) ?? throw new Exception("Правило не существует");

            if (!await CheckUserCanEditAsync(requestingUserID, networkRule.NetworkID))
            {
                throw new Exception("У вас нет доступу к редактированию правил в данной сети.");
            }

            if (networkRuleDto.RuleExpression == null && networkRuleDto.Rule == null)
            {
                throw new Exception("В правиле должно быть указано строковое правила, либо структурированное правило.");
            }

            networkRule.UserID = networkRuleDto.UserID;
            networkRule.NotificationType = networkRuleDto.NotificationType;
            networkRule.RuleExpression = networkRuleDto.RuleExpression ?? ParseRule(networkRuleDto.Rule!);


            await Context.SaveChangesAsync();

            await NotifyUserDataEventAsync(networkRule!, DataEventOperationType.Update);
        }

        public async Task DeleteNetworkRuleAsync(int requestingUserID, int networkRuleID)
        {
            var networkRule = await GetNetworkRuleAsync(networkRuleID) ?? throw new Exception("Правило не существует");

            if (!await CheckUserCanEditAsync(requestingUserID, networkRule.NetworkID))
            {
                throw new Exception("У вас нет доступа к удалению правил в данной сети.");
            }

            Context.Remove(networkRule);

            await Context.SaveChangesAsync();

            await NotifyUserDataEventAsync(networkRule!, DataEventOperationType.Delete);
        }

        #endregion

        #region Вспомогательное

        private IQueryable<Network> GetUserNetworks(int userID)
        {
            var activeNetworks = Context.Networks.Where(x => x.IsDeleted);
            var userNetworks = Context.NetworkUsers.Where(x => x.UserID == userID);

            var networksQuery = activeNetworks.Join(userNetworks,
                (n) => n.NetworkID,
                (u) => u.NetworkID,
                (n, u) => n);

            return networksQuery;
        }

        private IQueryable<Network> GetUserNetworks(int userID, Expression<Func<NetworkUser, bool>> filter)
        {
            var activeNetworks = Context.Networks.Where(x => x.IsDeleted);
            var userNetworks = Context.NetworkUsers.Where(x => x.UserID == userID).Where(filter);

            var networksQuery = activeNetworks.Join(userNetworks,
                (n) => n.NetworkID,
                (u) => u.NetworkID,
                (n, u) => n);

            return networksQuery;
        }

        private Task<NetworkRule?> GetNetworkRuleAsync(int networkRuleID)
        {
            return Context.NetworkRules.FirstOrDefaultAsync(x => x.NetworkRuleID == networkRuleID);
        }

        private Task<bool> CheckUserCanEditAsync(int userID, int networkID)
        {
            var networksQuery = GetUserNetworks(userID, x => x.IsEditor);

            return networksQuery.AnyAsync(x => x.NetworkID == networkID);
        }

        private async Task NotifyUserDataEventAsync(NetworkRule rule, DataEventOperationType operationType)
        {
            try
            {
                var dataEvent = new DataEventMessage<NetworkRuleDto>()
                {
                    Operation = operationType,
                    Data = Convert(rule)
                };

                await MessageSender.SendMessageAsync("BackendAll", dataEvent);
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, "Error on user event notification. Data may be lost.");
            }
        }

        private static NetworkRuleDto Convert(NetworkRule networkRule)
        {
            return new NetworkRuleDto()
            {
                NetworkRuleID = networkRule.NetworkRuleID,
                NetworkID = networkRule.NetworkID,
                UserID = networkRule.UserID,
                NotificationType = networkRule.NotificationType,
                RuleExpression = networkRule.RuleExpression,
                Rule = ParseExpression(networkRule.RuleExpression)
            };
        }

        private static string? ParseRule(RuleItemDto? ruleItem)
        {
            return null;
        }

        private static RuleItemDto? ParseExpression(string? expression)
        {
            return null;
        }

        #endregion
    }
}