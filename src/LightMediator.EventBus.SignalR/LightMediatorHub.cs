namespace LightMediator.EventBus.SignalR
{
    public class LightMediatorHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            return base.OnDisconnectedAsync(exception);
        }
        public async Task NewEventRaised(string newEvent)
        {
            await Clients.Others.SendAsync("EventRecieved", newEvent);
        }
    }
}
