using BackendCommonLibrary.Interfaces.Listeners;
using BackendCommonLibrary.Interfaces.Senders;
using System.Text.Json;

namespace BackendCommonLibrary.Extensions
{
    public static class MessagesQueueExtensions
    {
        public static Task SendMessageAsync<T>(this IMessageSender sender, string endpoint, T message)
        {
            var param = new Dictionary<string, string>()
            {
                { "content_type", typeof(T).Name }
            };

            var messageText =   JsonSerializer.Serialize(message);

            return sender.SendMessageAsync(endpoint, messageText, param);
        }

        public static void AddHandler(this IMessageListener listener, Action<string, string> onMessage)
        {
            listener.AddHandler(new StringMessageHandler()
            {
                OnMessage = onMessage
            });
        }

        public static void AddHandler<T>(this IMessageListener listener, Action<string, T> onMessage)
        {
            listener.AddHandler(new SerializeMessageHandler<T>()
            {
                OnMessage = onMessage
            });
        }

        private class StringMessageHandler : IMessageHandler
        {
            public required Action<string, string> OnMessage { get; set; }

            public void OnMessageRecieved(string queueName, string message)
            {
                OnMessage.Invoke(queueName, message);
            }
        }

        private class SerializeMessageHandler<T> : IMessageHandler
        {
            public required Action<string, T> OnMessage { get; set; }

            public void OnMessageRecieved(string queueName, string message)
            {
                //var content = 

                //OnMessage.Invoke(queueName, def);
            }
        }
    }
}

