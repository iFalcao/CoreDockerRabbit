namespace PublisherApi.Services
{
    public interface IMessageService
    {
        bool Enqueue(string message);
    }
}
