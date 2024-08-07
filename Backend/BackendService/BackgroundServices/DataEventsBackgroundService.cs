using BackendCommonLibrary.Interfaces.BackgroundServices;

namespace BackendService.BackgroundServices
{
    public class DataEventsBackgroundService : BackgroundService
    {
        private IServiceProvider ServiceProvider { get; set; }

        private IConfiguration Configuration { get; set; }

        private ILogger Logger { get; set; }


        public DataEventsBackgroundService(IServiceProvider serviceProvider, IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            ServiceProvider = serviceProvider;
            Configuration = configuration;
            Logger = loggerFactory.CreateLogger<DataEventsBackgroundService>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                Logger.LogInformation("Сервис начал работу.");

                using var scope = ServiceProvider.CreateScope();

                var service = scope.ServiceProvider.GetRequiredService<IDataEventsService>();

                await service.WorkAsync(stoppingToken);

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
    }
}
