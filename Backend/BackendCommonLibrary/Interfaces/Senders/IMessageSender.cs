namespace BackendCommonLibrary.Interfaces.Senders
{
    public interface IMessageSender
    {
        Task SendMessageAsync(string endpoint, string message, Dictionary<string, string>? param = null);
    }
}