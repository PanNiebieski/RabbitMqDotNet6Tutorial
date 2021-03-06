using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


while (true)
{

    Console.WriteLine("RPC Client");
    string n = args.Length > 0 ? args[0] : "30";
    Task t = InvokeAsync(n);
    await t;
    Console.WriteLine(" Press [enter] to exit.");
    Console.ReadKey();
}



Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();







static async Task InvokeAsync(string n)
{
    var rnd = new Random(Guid.NewGuid().GetHashCode());
    var rpcClient = new RpcClient();

    Console.WriteLine(" [x] Requesting fib({0})", n);
    var response = await rpcClient.CallAsync(n.ToString());
    Console.WriteLine(" [.] Got '{0}'", response);

    rpcClient.Close();
}



class RpcClient
{
    private const string QUEUE_NAME = "rpc_queue";

    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly string _replyQueueName;
    private readonly EventingBasicConsumer _consumer;
    private readonly ConcurrentDictionary<string,
        TaskCompletionSource<string>> _callbackMapper =
                new ConcurrentDictionary<string, TaskCompletionSource<string>>();

    public RpcClient()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _replyQueueName = _channel.QueueDeclare().QueueName;
        _consumer = new EventingBasicConsumer(_channel);
        _consumer.Received += (model, ea) =>
        {
            if (!_callbackMapper.TryRemove(ea.BasicProperties.CorrelationId, out TaskCompletionSource<string> tcs))
            {
                return;
            }
            var response = Encoding.UTF8.GetString(ea.Body.ToArray());
            tcs.TrySetResult(response);
        };
    }

    public Task<string> CallAsync
        (string message, CancellationToken cancellationToken = default(CancellationToken))
    {
        IBasicProperties props = _channel.CreateBasicProperties();
        var correlationId = Guid.NewGuid().ToString();
        props.CorrelationId = correlationId;
        props.ReplyTo = _replyQueueName;
        var messageBytes = Encoding.UTF8.GetBytes(message);
        var tcs = new TaskCompletionSource<string>();
        _callbackMapper.TryAdd(correlationId, tcs);

        _channel.BasicPublish(
            exchange: "",
            routingKey: QUEUE_NAME,
            basicProperties: props,
            body: messageBytes);

        _channel.BasicConsume(
            consumer: _consumer,
            queue: _replyQueueName,
            autoAck: true);

        cancellationToken.Register(() => _callbackMapper.
        TryRemove(correlationId, out var tmp));

        return tcs.Task;
    }

    public void Close()
    {
        _connection.Close();
    }
}
