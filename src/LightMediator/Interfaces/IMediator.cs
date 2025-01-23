namespace LightMediator;

public interface IMediator
{ 
    Task Publish(INotification notification, CancellationToken? cancellationToken = null);
}   
