using RabbitMQ.Client;
using System.Text;


var factory = new ConnectionFactory() { HostName = "localhost" };
using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
    channel.ExchangeDeclare(exchange: "RabbitMqDotNet6Tutorial.05",
                            type: ExchangeType.Topic);

    Console.WriteLine("Write routingKey:");

    string usermessageRoutingKey = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(usermessageRoutingKey))
    {
        Environment.ExitCode = 1;
        return;
    }

    var message = "Daj komentarz";

    var body = Encoding.UTF8.GetBytes(message);
    channel.BasicPublish(exchange: "RabbitMqDotNet6Tutorial.05",
                         routingKey: usermessageRoutingKey,
                         basicProperties: null,
                         body: body);

    Console.WriteLine(" [x] Sent '{0}':'{1}'", usermessageRoutingKey, message);
}