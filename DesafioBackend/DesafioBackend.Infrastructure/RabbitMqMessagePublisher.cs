using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace DesafioBackend.Infrastructure;

public class RabbitMqMessagePublisher : IMessagePublisher, IAsyncDisposable
{
    private IConnection _connection;
    private IChannel _channel;

    public async Task InitializeAsync(string hostname = "localhost")
    {
        var factory = new ConnectionFactory
        {
            HostName = hostname
        };

        _connection = await factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();
    }

    public async Task PublishAsync(string routingKey, object message)
    {
        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        await _channel.QueueDeclareAsync(
            queue: routingKey,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        await _channel.BasicPublishAsync(
            exchange: "",
            routingKey: routingKey,
            mandatory: false,
            body: body
        );
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel != null)
            await _channel.CloseAsync();

        if (_connection != null)
            await _connection.CloseAsync();
    }
}
