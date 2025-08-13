using Microsoft.EntityFrameworkCore.Metadata;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace DesafioBackend.Infrastructure;

public class RabbitMqMessagePublisher : IMessagePublisher, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMqMessagePublisher(string hostname = "localhost")
    {
        var factory = new ConnectionFactory() { HostName = hostname };
        // Aqui vamos aguardar a criação assíncrona da conexão
        _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
        // Também para o canal
        _channel = _connection.CreateModelAsync().GetAwaiter().GetResult();
    }

    public Task PublishAsync(string routingKey, object message)
    {
        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        _channel.QueueDeclare(queue: routingKey,
                              durable: false,
                              exclusive: false,
                              autoDelete: false,
                              arguments: null);

        _channel.BasicPublish(exchange: "",
                              routingKey: routingKey,
                              basicProperties: null,
                              body: body);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
    }
}


