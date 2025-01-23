namespace ServiceA.BackService.Application.Notifications;

public class NewAppInfoNotification:INotification
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
}
