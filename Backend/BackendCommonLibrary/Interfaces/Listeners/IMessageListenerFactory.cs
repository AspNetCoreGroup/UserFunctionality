namespace BackendCommonLibrary.Interfaces.Listeners
{
    public interface IMessageListenerFactory : IDisposable
    {
        IMessageListener CreateListener(string queue);
    }
}