using BackendCommonLibrary.Interfaces.Listeners;

namespace BackendService.Listeners
{
    public sealed class RabbitListenerFactory : IMessageListenerFactory
    {
        private ILogger Logger { get; set; }

        private ILoggerFactory LoggerFactory { get; set; }

        private IConfiguration Configuration { get; set; }

        private LinkedList<RabbitListener> Listeners { get; set; }


        public RabbitListenerFactory(ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            Logger = loggerFactory.CreateLogger<RabbitListenerFactory>();
            LoggerFactory = loggerFactory;
            Configuration = configuration;
            Listeners = new LinkedList<RabbitListener>();
        }

        public IMessageListener CreateListener(string queue)
        {
            var listener = new RabbitListener(Configuration, queue);

            Listeners.AddLast(listener);

            return listener;
        }

        public void Dispose()
        {
            foreach (var listener in Listeners)
            {
                listener.Dispose();
            }
        }
    }
}