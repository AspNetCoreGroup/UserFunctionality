using System;
using BackendCommonLibrary.Extensions;
using BackendCommonLibrary.Interfaces.Senders;
using BackendCommonLibrary.Interfaces.Listeners;
using BackendCommonLibrary.Interfaces.Services;
using System.Text.Json;

namespace BackendService.Services
{
    public class MessagesQueueRequestsService : IRequestsService
    {
        private IMessageSender MessageSender { get; set; }

        private IMessageListenerFactory MessageListenerFactory { get; set; }


        public MessagesQueueRequestsService(IMessageSender messageSender, IMessageListenerFactory messageListenerFactory)
        {
            MessageSender = messageSender;
            MessageListenerFactory = messageListenerFactory;
        }

        public async Task SendRequestAsync<T>(string path, T argument)
        {
            var argumentStr = JsonSerializer.Serialize(argument);

            await MessageSender.SendMessageAsync(path, argumentStr);
        }

        public async Task<V> SendRequestAsync<V, T>(string path, T argument)
        {
            var argumentStr = JsonSerializer.Serialize(argument);
            await MessageSender.SendMessageAsync(path, argumentStr);

            using var listener = MessageListenerFactory.CreateListener(path);
            var resultCatcher = new ResultCatcher<string>();

            listener.AddHandler((q, c) =>
            {
                resultCatcher.SetResult(c);
            });

            listener.Open();

            var resultStr = await resultCatcher.GetResultAsync(5000);
            var result = JsonSerializer.Deserialize<V>(resultStr) ?? throw new JsonException($"Ошибка парсинга {typeof(V).Name} - {resultStr}");

            return result;
        }

        private class ResultCatcher<T> where T : class
        {
            private T? Result { get; set; }

            public void SetResult(T value)
            {
                Result = value;
            }

            public async Task<T> GetResultAsync(int timeout)
            {
                var task = GetResultAsync();

                if (await Task.WhenAny(task, Task.Delay(timeout)) == task)
                {
                    return task.Result;
                }
                else
                {
                    throw new TimeoutException();
                }
            }

            private async Task<T> GetResultAsync()
            {
                while (Result == null)
                {
                    await Task.Delay(100);
                }

                return Result;
            }
        }
    }
}