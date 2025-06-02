
using System.Threading;

namespace LightMediator.EventBus.MQTT;

internal class MQTTEventBus : IMQTTEventBus, IHostedService
{
    private readonly IMediator _mediator;
    private readonly ILogger<MQTTEventBus> _logger;
    private readonly IHostApplicationLifetime _lifetime;
    private readonly IMqttClient _client;
    private readonly MQTTSettings _settings;
    internal readonly LightMediatorOptions _mediatorOptions;
    private readonly MQTTEventBusOptions _mQTTEventBusOptions;
    private readonly IServiceProvider _serviceProvider;
    private MqttClientOptions? _options;
    private EventBusConnectionState _clientState;
     

    public MQTTEventBus(
        IMediator mediator,
        ILogger<MQTTEventBus> logger,
        LightMediatorOptions mediatorOptions,
        IMqttClient client,
        IHostApplicationLifetime lifetime,
        MQTTSettings settings,
        MQTTEventBusOptions mQTTEventBusOptions,
        IServiceProvider serviceProvider)
    {
        _mediator = mediator;
        _logger = logger;
        _mediatorOptions = mediatorOptions;
        _client = client;
        _lifetime = lifetime;
        _settings = settings;
        _mQTTEventBusOptions = mQTTEventBusOptions;
        _serviceProvider = serviceProvider;
    }

    public async Task OnEventRecieved(string notificationMessage, CancellationToken? cancellationToken)
    {
        try
        {
            var MQTTMQEvent = JsonConvert.DeserializeObject<MQTTEvent>(notificationMessage)
                               ?? throw new EventDeserializationException("Failed to deserialize MQTTEvent.");
            var currentAssembly = Assembly.GetEntryAssembly()?.FullName;
            if (MQTTMQEvent.AssemblyName == currentAssembly)
                return;
            var t = _mediatorOptions.Assemblies.SelectMany(a => a.GetTypes())
                .FirstOrDefault(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith(MQTTMQEvent.TypeName, StringComparison.Ordinal));
            if (t == null)
                throw new EventDeserializationException("The type not found in referenced assemblies");
            var notification = (INotification?)JsonConvert.DeserializeObject(MQTTMQEvent.JsonPayload, t);
            if (notification == null)
                throw new EventDeserializationException("Failed to deserialize payload to INotification.");

            await _mediator.Publish(notification, cancellationToken ?? CancellationToken.None);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process MQTT event.");
            throw;
        }
    }

    public async Task PublishAsync(INotification notification, params string[] extraTopics)
    {
        var eventMessage = new MQTTEvent(
            notification.GetType().Name.Split(".").Last(),
            JsonConvert.SerializeObject(notification),
            Assembly.GetEntryAssembly()!.FullName!
        );
        if (extraTopics == null)
        {
            await PublishMqttMessage(eventMessage, $"lightmediator-events");
        }
        else
        {
            foreach (var topic in extraTopics)
            {
                await PublishMqttMessage(eventMessage, topic);
            }
        }

    }
    public async Task PublishAsync(INotification notification)
    {
        if (_settings.IsDefaultEventBus)
        {
            var eventMessage = new MQTTEvent(
                notification.GetType().Name.Split(".").Last(),
                JsonConvert.SerializeObject(notification),
                Assembly.GetEntryAssembly()!.FullName!
            );
            await PublishMqttMessage(eventMessage, $"lightmediator-events");
        }
    }

    private async Task PublishMqttMessage(MQTTEvent eventMessage, string topic)
    {
        var message = new MqttApplicationMessageBuilder()
                     .WithTopic(topic)
                     .WithPayload(JsonConvert.SerializeObject(eventMessage))
                     .Build();
        if (_client.IsConnected)
            await _client.PublishAsync(message);
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (_options == null)
            await BuildOptions();
        _lifetime.ApplicationStarted.Register(() =>
        {
            _ = Task.Run(async () =>
            {
                await ConnectAsync(cancellationToken);
            });
        });
    }
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _client.DisconnectAsync(new MqttClientDisconnectOptions() { }, cancellationToken);
    }

    private async Task BuildOptions()
    {

        var optionsBuilder = new MqttClientOptionsBuilder();
        if (_settings.UseWebSocket)
        {
            optionsBuilder.WithWebSocketServer(c => c.WithUri($"wss://{_settings.WssServerAddress}:{_settings.WssPortNumber}/mqtt"));
        }
        else
        {
            optionsBuilder.WithTcpServer(_settings.ServerAddress, _settings.PortNumber);
        }
        if (_settings.UseTls)
        {
            optionsBuilder.WithTlsOptions(o =>
            {
                o.UseTls(true);
            });
        }
        if (!string.IsNullOrEmpty(_settings.ClientId))
            optionsBuilder.WithClientId(_settings.ClientId);
        if (_mQTTEventBusOptions.TokenHandlerType is not null)
        {
            var mQTTTokenHandler = _serviceProvider.GetService<IMQTTTokenHandler>();
            if (mQTTTokenHandler == null)
                throw new MQTTConfigurationException("No IMQTTTokenHandler registered");
            var tokenInfo = await mQTTTokenHandler.GetTokenInfoAsync(token =>
            {
                if (OnTokenChanged == null)
                    OnTokenChanged = async (token) =>
                    {
                        if (_client.IsConnected)
                        {
                            await _client.DisconnectAsync();
                            _client.ConnectedAsync -= MqttConnected;
                            _client.DisconnectedAsync -= MqttDisconnected;
                            _client.ConnectingAsync -= MqttConnecting;
                            _client.ApplicationMessageReceivedAsync -= MessageRecieved;
                        }
                        await BuildOptions();
                        _clientState = EventBusConnectionState.Reconncting;
                        await ConnectAsync(default);
                    };
                return OnTokenChanged(token);
            });
            if (tokenInfo != null)
            { 
                _settings.Username = tokenInfo.username;
                if ( _settings.SendTokenInPassword)
                {
                    optionsBuilder.WithCredentials(tokenInfo.username, tokenInfo.token);
                }
                else if ( !_settings.SendTokenInPassword)
                {
                    optionsBuilder.WithCredentials(tokenInfo.token, tokenInfo.username);
                }
            }
        }
        else
        {
            if (!string.IsNullOrEmpty(_settings.Username) || !string.IsNullOrEmpty(_settings.Password))
                optionsBuilder.WithCredentials(_settings.Username, _settings.Password);
        }
        _options = optionsBuilder.Build();
    }
    Func<string, Task>? OnTokenChanged { get; set; }
    private async Task ConnectAsync(CancellationToken cancellationToken)
    {
        try
        {
            int c = 1;
            while (!_client.IsConnected)
            {
                var delay = CalculateDelay(c);
                await Task.Delay(delay, cancellationToken);

                _logger.LogInformation("------------------------------------------Connecting to MQTT");
                var res = await _client.ConnectAsync(_options, cancellationToken);
                _logger.LogInformation($"_+_+_+_+_+_+_+_+_+__+_+_+_+_+_+_+_+ {res.ResultCode}");
                c++;
                if (!_client.IsConnected)
                    continue;
                _logger.LogInformation($"-----------------------MQTT Connected----------------------CLIENTID:{_settings.ClientId}");

                
                _client.ConnectedAsync += MqttConnected;
                _client.DisconnectedAsync += MqttDisconnected;
                _client.ConnectingAsync += MqttConnecting;
                _client.ApplicationMessageReceivedAsync += MessageRecieved;

                if (_settings.IsDefaultEventBus) 
                    await _client.SubscribeAsync(_settings.LightmediatorTopicName);

                if (_settings.SubscribeToUsernameTopic)
                    await _client.SubscribeAsync(_settings.Username);

                if (_mQTTEventBusOptions.ExtraTopics.Any())
                    foreach (var topic in _mQTTEventBusOptions.ExtraTopics)
                    {
                        await _client.SubscribeAsync(topic);
                    }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to connect to MQTT. Retrying in 5 seconds...");
        }
    }

    private Task MqttConnecting(MqttClientConnectingEventArgs args)
    {
        _clientState = EventBusConnectionState.Connecting;
        return Task.CompletedTask;
    }

    private int CalculateDelay(int attempt)
    {
        const int initialDelayMs = 1000;
        const int maxDelayMs = 30000;
        const double factor = 2.0;

        int delay = (int)(initialDelayMs * Math.Pow(factor, attempt - 1));

        var jitter = new Random().Next(0, 500);

        return Math.Min(delay + jitter, maxDelayMs);
    }


    private async Task MessageRecieved(MqttApplicationMessageReceivedEventArgs args)
    {
        var message = args.ApplicationMessage.ConvertPayloadToString();
        await OnEventRecieved(message, null);
    }

    private async Task MqttDisconnected(MqttClientDisconnectedEventArgs args)
    {
        if (_settings.AutoReconenctOnDisconnection)
        {
            if (_clientState != EventBusConnectionState.Reconncting)
            {
                await Task.Delay(5000);

                await ConnectAsync(default);
            }
        }
        else
        {
            _logger.LogInformation("Disconnected from MQTT broker.");
            _clientState = EventBusConnectionState.Disconnected;

        }
    }
    private Task MqttConnected(MqttClientConnectedEventArgs args)
    {
        _logger.LogInformation("Connected to MQTT broker.");
        _clientState = EventBusConnectionState.Connected;
        return Task.CompletedTask;
    }

}
