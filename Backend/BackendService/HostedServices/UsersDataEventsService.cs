using CommonLibrary.Extensions;
using CommonLibrary.Interfaces.Listeners;
using CommonLibrary.Interfaces.Services;
using ModelLibrary.Messages;
using ModelLibrary.Model;
using ModelLibrary.Model.Enums;
using System.Text.Json;

namespace BackendService.HostedServices
{
    public class UsersDataEventsService : BackgroundService
    {
        private IMessageListenerFactory MessageListenerFactory { get; set; }
        private ILogger Logger { get; set; }
        private IUsersService UsersService { get; set; }

        public UsersDataEventsService(IMessageListenerFactory messageListenerFactory, ILoggerFactory loggerFactory, IUsersService usersService)
        {
            MessageListenerFactory = messageListenerFactory;
            Logger = loggerFactory.CreateLogger<UsersDataEventsService>();
            UsersService = usersService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                Logger.LogInformation("Сервис запущен.");

                using var listener = MessageListenerFactory.CreateListener("AuthorizationBackend");

                listener.AddHandler(ProcessMessage);

                listener.Open();

                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(200, stoppingToken);
                }

                await Task.CompletedTask;

                Logger.LogInformation("Сервис завершил работу.");
            }
            catch (TaskCanceledException)
            {
                Logger.LogInformation("Сервис завершил работу.");
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, "Ошибка при запуске фонового сервиса.");
            }
        }

        private void ProcessMessage(string queueName, string message, Dictionary<string, string> param)
        {
            try
            {
                Logger.LogInformation("Получено сообщение \"{queueName}\"  - \"{message}\".", queueName, message);

                var contentType = param["ContentType"];

                if (!contentType.StartsWith("DataEventMessage"))
                {
                    throw new Exception("В очереди изменений разрешается использовать только сообщения с типом DataEventMessage");
                }

                if (contentType == "DataEventMessage<UserDto>")
                {
                    var dataEvent = DeserializeMessage<DataEventMessage<UserDto>>(message);
                    ProcessUserMessage(dataEvent).Wait();
                }

                Logger.LogInformation("Обработано сообщение \"{queueName}\"  - \"{message}\".", queueName, message);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Ошибка при обработке сообщения \"{queueName}\"  - \"{message}\".", queueName, message);
            }
        }

        private T DeserializeMessage<T>(string message)
        {
            return JsonSerializer.Deserialize<T>(message) ?? throw new Exception($"Ошибка десериализации \"{message}\" в {nameof(T)}");
        }

        private async Task ProcessUserMessage(DataEventMessage<UserDto> userDataEvent)
        {
            if (userDataEvent.Operation == DataEventOperationType.Add)
            {
                await UsersService.CreateUserAsync(userDataEvent.Data!);
            }
            else if (userDataEvent.Operation == DataEventOperationType.Update)
            {
                await UsersService.UpdateUserAsync(userDataEvent.Data!.UserID, userDataEvent.Data!);
            }
            else if (userDataEvent.Operation == DataEventOperationType.Delete)
            {
                await UsersService.DeleteUserAsync(userDataEvent.Data!.UserID);

            }
        }
    }
}
