using System.Globalization;
using MQTTnet;
using MQTTnet.Client;

namespace HomeApi.Services;

public class MqttService : BackgroundService
{
    private readonly ILogger<MqttService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IMqttClient _client;
    private readonly MqttFactory _factory;

    public MqttService(ILogger<MqttService> logger,
        IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
        
        _factory = new MqttFactory();
        _client = _factory.CreateMqttClient();

        _client.ApplicationMessageReceivedAsync += messageReceived;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            var clientOptions = new MqttClientOptionsBuilder()
                .WithTcpServer("10.0.0.5")
                .Build();

            await _client.ConnectAsync(clientOptions, stoppingToken);

            var subscribeOptions = _factory.CreateSubscribeOptionsBuilder()
                .WithTopicFilter(f =>
                {
                    f.WithTopic("house/#");
                })
                .Build();

            await _client.SubscribeAsync(subscribeOptions, stoppingToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error connecting to mqtt server: {e.Message}");
            throw;
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _client.DisconnectAsync(
            new MqttClientDisconnectOptionsBuilder()
                .WithReason(MqttClientDisconnectOptionsReason.NormalDisconnection)
                .Build());
    }

    public override void Dispose()
    {
        base.Dispose();

        if (_client != null)
        {
            _client.ApplicationMessageReceivedAsync -= messageReceived;
        }
    }

    private async Task messageReceived(MqttApplicationMessageReceivedEventArgs args)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(args);
            ArgumentNullException.ThrowIfNull(args.ApplicationMessage);
            
            var topic = args.ApplicationMessage.Topic;
            var msg = args.ApplicationMessage.ConvertPayloadToString();
            
            _logger.LogInformation($"Received message from topic {topic} with payload {msg}");
            
            var location = topic.Split("/")[1];
            var items = msg.Split(";");
            
            var tempStr = items[0].Split("=")[1];
            var temp = Convert.ToDouble(tempStr, CultureInfo.InvariantCulture);
            
            var humStr = items[1].Split("=")[1];
            var hum = Convert.ToDouble(humStr, CultureInfo.InvariantCulture);

            using var scope = _serviceScopeFactory.CreateScope();
            var tempService = scope.ServiceProvider.GetRequiredService<ITempService>();
            await tempService.Add(location, temp, hum);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error when receiving mqtt message: {e.Message}");
            throw;
        }
    }
}