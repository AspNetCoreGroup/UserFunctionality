using BackendCommonLibrary.Interfaces.Senders;
using RabbitMQ.Client;
using System.Text;

namespace BackendService.Senders
{
    public class RabbitSender : IMessageSender
    {
        #region Поля

        private IConfiguration Configuration { get; }

        #endregion

        public RabbitSender(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public Task SendMessageAsync(string endpoint, string message, Dictionary<string, string>? param = null)
        {
            var endpointConf = Configuration.GetSection($"Senders/RabbitMQ/Endpoints/{endpoint}");

            var hostName = endpointConf["HostName"] ?? throw new NullReferenceException($"Для {endpoint} не указан HostName");
            var virtualHost = endpointConf["VirtualHost"] ?? throw new NullReferenceException($"Для {endpoint} не указан VirtualHost");
            var userName = endpointConf["UserName"] ?? throw new NullReferenceException($"Для {endpoint} не указан UserName");
            var password = endpointConf["Password"] ?? throw new NullReferenceException($"Для {endpoint} не указан Password");
            var queue = endpointConf["Queue"];
            var exchange = endpointConf["Exchange"];

            if (queue == null && exchange == null)
            {
                throw new NullReferenceException($"Для {endpoint} не указан Queue или Exchange");
            }

            var factory = new ConnectionFactory
            {
                HostName = hostName,
                VirtualHost = virtualHost,
                UserName = userName,
                Password = password
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: queue ?? "", durable: false, exclusive: false, autoDelete: false, arguments: null);

            var body = Encoding.UTF8.GetBytes(message);

            var properties = channel.CreateBasicProperties();

            channel.BasicPublish(exchange: exchange ?? "", routingKey: null, basicProperties: null, body: body);

            return Task.CompletedTask;
        }
    }
}