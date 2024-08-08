using System.Text.Json;
using BackendCommonLibrary.Interfaces.BackgroundServices;
using BackendCommonLibrary.Interfaces.Services;
using CommonLibrary.Extensions;
using CommonLibrary.Interfaces.Listeners;
using ModelLibrary.Messages;
using ModelLibrary.Model;
using ModelLibrary.Model.Enums;

namespace BackendService.BackgroundServices
{
    public class DataEventsService : IDataEventsService
    {
        private IConfiguration Configuration { get; }

        private ILogger Logger { get; }

        private IUsersService UsersService { get; }

        private IDevicesService DevicesService { get; }

        private IMessageListenerFactory MessageListenerFactory { get; }


        public DataEventsService(IConfiguration configuration, IDevicesService devicesService, IUsersService usersService, IMessageListenerFactory messageListenerFactory, ILoggerFactory loggerFactory)
        {
            Configuration = configuration;
            UsersService = usersService;
            DevicesService = devicesService;
            MessageListenerFactory = messageListenerFactory;
            Logger = loggerFactory.CreateLogger<DataEventsBackgroundService>();
        }

        public async Task WorkAsync(CancellationToken stoppingToken)
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

                if (contentType == "DataEventMessage<DeviceDto>")
                {
                    var dataEvent = DeserializeMessage<DataEventMessage<DeviceDto>>(message);
                    ProcessDeviceMessage(dataEvent).Wait();
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

        private async Task ProcessDeviceMessage(DataEventMessage<DeviceDto> deviceDataEvent)
        {
            if (deviceDataEvent.Operation == DataEventOperationType.Add)
            {
                await DevicesService.CreateDeviceAsync(deviceDataEvent.Data!);
            }
            else if (deviceDataEvent.Operation == DataEventOperationType.Update)
            {
                await DevicesService.UpdateDeviceAsync(deviceDataEvent.Data!.DeviceID, deviceDataEvent.Data!);
            }
            else if (deviceDataEvent.Operation == DataEventOperationType.Delete)
            {
                await DevicesService.DeleteDeviceAsync(deviceDataEvent.Data!.DeviceID);
            }
        }
    }
}