using System.Text.Json;
using BackendCommonLibrary.Interfaces.BackgroundServices;
using BackendCommonLibrary.Interfaces.Services;
using CommonLibrary.Extensions;
using CommonLibrary.Interfaces.Listeners;
using ModelLibrary.Messages;
using ModelLibrary.Model;
using ModelLibrary.Events;
using ModelLibrary.Model.Enums;
using BackendService.Model;

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
            Logger = loggerFactory.CreateLogger<DataEventsService>();
        }

        public async Task WorkAsync(CancellationToken stoppingToken)
        {
            try
            {
                Logger.LogInformation("Запуск сервиса");

                using var listener = MessageListenerFactory.CreateListener("DataEventsBackend");

                listener.AddHandler(ProcessMessage);

                listener.Open();

                Logger.LogInformation("Сервис запущен");

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

        private void ProcessMessage(MessageRecievedEventArgs args)
        {
            var message = args.Message;
            var queueName = args.QueueName;
            var param = args.Param;

            try
            {
                Logger.LogInformation("Получено сообщение \"{queueName}\" - \"{message}\".", queueName, message);

                var contentType = param["ContentType"];

                //if (!contentType.StartsWith("DataEventMessage"))
                //{
                //    throw new Exception("В очереди изменений разрешается использовать только сообщения с типом DataEventMessage");
                //}

                if (contentType == "DataEventMessage<UserDto>")
                {
                    var dataEvent = DeserializeMessage<DataEventMessage<UserDto>>(message);
                    ProcessUserMessage(dataEvent).Wait();
                }

                if (contentType == "DeviceMessage")
                {
                    var dataEvent = DeserializeMessage<DeviceMessage>(message);
                    ProcessDeviceMessage(dataEvent).Wait();
                }

                args.Hadled = true;

                Logger.LogInformation("Обработано сообщение \"{queueName}\" - \"{message}\".", queueName, message);
            }
            catch (Exception e)
            {
                args.Rejected = true;

                Logger.LogError(e, "Ошибка при обработке сообщения \"{queueName}\" - \"{message}\".", queueName, message);
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

        private async Task ProcessDeviceMessage(DeviceMessage deviceMessage)
        {
            var device = new DeviceDto()
            {
                DeviceID = deviceMessage.DeviceId,
                DeviceCode = deviceMessage.SerialNumber,
                DeviceCustomTitle = deviceMessage.DeviceType
            };

            await DevicesService.CreateDeviceAsync(device);
        }
    }
}