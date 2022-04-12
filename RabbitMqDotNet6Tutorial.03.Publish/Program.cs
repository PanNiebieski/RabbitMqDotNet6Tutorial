using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory() { HostName = "localhost" };

using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
    channel.ExchangeDeclare(exchange: "RabbitMqDotNet6Tutorial.03", type: ExchangeType.Fanout,
        durable: true, autoDelete: false, arguments: null);

    while (true)
    {
        var message = GetMessage(args);
        var body = Encoding.UTF8.GetBytes(message);
        channel.BasicPublish(exchange: "RabbitMqDotNet6Tutorial.03", routingKey: "",
            basicProperties: null, body: body);
        Console.WriteLine(" [x] Sent {0}", message);
        Console.ReadLine();
    }

}

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();


static string GetMessage(string[] args)
{
    return ((args.Length > 0) ? string.Join(" ", args) : "info: Hello World!");
}
