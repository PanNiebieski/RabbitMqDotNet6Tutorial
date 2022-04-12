using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

Console.Title = "RabbitMqDotNet6Tutorial.01.Receive";
Console.WriteLine("RabbitMqDotNet6Tutorial.01.Receive");

var factory = new ConnectionFactory() { HostName = "localhost" };

using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
    channel.QueueDeclare(queue: "RabbitMqDotNet6Tutorial.01", durable: false, 
        exclusive: false, autoDelete: false, arguments: null);

    Console.WriteLine(" Waiting for messages.");

    var consumer = new EventingBasicConsumer(channel);

    consumer.Received += (model, ea) =>
    {
        var message = Encoding.UTF8.GetString(ea.Body.ToArray());
        Console.Write("-> Received: ");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(message);
        Console.ForegroundColor = ConsoleColor.Gray;

        //channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
    };
    channel.BasicConsume(queue: "RabbitMqDotNet6Tutorial.01",
        autoAck: true, consumer: consumer);


    char key = 'z';

    while (key != 'q' && key != 'Q')
    {
        Console.WriteLine(" Press [q] or [Q] to exit.");
        key = Console.ReadKey().KeyChar;
    }

}


