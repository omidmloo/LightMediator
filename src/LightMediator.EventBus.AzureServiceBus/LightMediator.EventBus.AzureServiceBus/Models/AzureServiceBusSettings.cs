namespace LightMediator.EventBus.AzureServiceBus.Models;

public class AzureServiceBusSettings
{
    public string ConnectionString { get; set; } = string.Empty;  
    public bool EnableRetry { get; set; } = true; 
    public int RetryCount { get; set; } = 3;
    public int RetryIntervalMs { get; set; } = 1000; 
    public string? TopicName { get; set; }
}

