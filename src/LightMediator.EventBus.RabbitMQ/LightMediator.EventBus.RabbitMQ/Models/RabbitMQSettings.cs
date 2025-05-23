namespace LightMediator.EventBus.RabbitMQ.Models;

public class RabbitMQSettings
{
    /// <summary>
    /// RabbitMQ host URI (e.g., "rabbitmq://localhost")
    /// </summary>
    public string HostUri { get; set; } = "rabbitmq://localhost";

    /// <summary>
    /// Username for RabbitMQ connection
    /// </summary>
    public string Username { get; set; } = "guest";

    /// <summary>
    /// Password for RabbitMQ connection
    /// </summary>
    public string Password { get; set; } = "guest";

    /// <summary>
    /// Virtual host to connect to (default "/")
    /// </summary>
    public string VirtualHost { get; set; } = "/"; 

    /// <summary>
    /// Enable message retry (MassTransit built-in policy)
    /// </summary>
    public bool EnableRetry { get; set; } = true;

    /// <summary>
    /// Number of retry attempts
    /// </summary>
    public int RetryCount { get; set; } = 3;

    /// <summary>
    /// Retry interval in milliseconds
    /// </summary>
    public int RetryIntervalMs { get; set; } = 1000;
}
