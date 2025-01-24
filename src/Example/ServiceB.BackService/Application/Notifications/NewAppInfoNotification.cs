namespace ServiceB.BackService.Application.Notifications;

public class NewAppInfoNotification : INotification
{
    public string Title { get; set; }
    public string? Description { get; set; }
}
