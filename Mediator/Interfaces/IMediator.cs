namespace AppMediator;

public interface IMediator
{ 
    Task Publish(INotification notification, CancellationToken? cancellationToken = null);
}   
