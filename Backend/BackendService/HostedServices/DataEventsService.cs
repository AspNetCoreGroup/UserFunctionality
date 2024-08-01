using System.Text.Json;
using BackendCommonLibrary.Interfaces.Services;
using CommonLibrary.Extensions;
using CommonLibrary.Interfaces.Listeners;
using ModelLibrary.Messages;
using ModelLibrary.Model;
using ModelLibrary.Model.Enums;

namespace BackendService.HostedServices
{
    public class DataEventsService : BackgroundService
    {
        private IServiceProvider ServiceProvider { get; set; }

        private ILogger Logger { get; set; }



        public DataEventsService(IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
        {
            ServiceProvider = serviceProvider;
            Logger = loggerFactory.CreateLogger<DataEventsService>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                Logger.LogInformation("Сервис запущен.");

                using var listenerFactory = ServiceProvider.GetService<IMessageListenerFactory>()!;
                using var listener = listenerFactory.CreateListener("AuthorizationBackend");

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
            var usersService = ServiceProvider.GetService<IUsersService>()!;

            if (userDataEvent.Operation == DataEventOperationType.Add)
            {
                await usersService.CreateUserAsync(userDataEvent.Data!);
            }
            else if (userDataEvent.Operation == DataEventOperationType.Update)
            {
                await usersService.UpdateUserAsync(userDataEvent.Data!.UserID, userDataEvent.Data!);
            }
            else if (userDataEvent.Operation == DataEventOperationType.Delete)
            {
                await usersService.DeleteUserAsync(userDataEvent.Data!.UserID);
            }
        }

        private async Task ProcessDeviceMessage(DataEventMessage<DeviceDto> deviceDataEvent)
        {
            var devicesService = ServiceProvider.GetService<IDevicesService>()!;
            
            if (deviceDataEvent.Operation == DataEventOperationType.Add)
            {
                await devicesService.CreateDeviceAsync(deviceDataEvent.Data!);
            }
            else if (deviceDataEvent.Operation == DataEventOperationType.Update)
            {
                await devicesService.UpdateDeviceAsync(deviceDataEvent.Data!.DeviceID, deviceDataEvent.Data!);
            }
            else if (deviceDataEvent.Operation == DataEventOperationType.Delete)
            {
                await devicesService.DeleteDeviceAsync(deviceDataEvent.Data!.DeviceID);
            }
        }
    }
}
