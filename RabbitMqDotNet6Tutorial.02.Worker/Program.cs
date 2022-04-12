using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

Console.Title = "RabbitMqDotNet6Tutorial.02.Worker";
Console.WriteLine("RabbitMqDotNet6Tutorial.02.Worker");


Console.ForegroundColor = GetRandomConsoleColor();
var factory = new ConnectionFactory() { HostName = "localhost" };
using (var connection = factory.CreateConnection())

using (var channel = connection.CreateModel())
{
    channel.QueueDeclare(queue: "RabbitMqDotNet6Tutorial.02", durable: true, exclusive: false, autoDelete: false, arguments: null);

    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

    Console.WriteLine(" [*] Waiting for jobs.");

    var consumer = new EventingBasicConsumer(channel);

    consumer.Received += (model, ea) =>
    {
        byte[] body = ea.Body.ToArray();
        var jobAsJsonText = Encoding.UTF8.GetString(body);

        Job job = JsonSerializer.Deserialize<Job>(jobAsJsonText);

        Console.WriteLine(" [>] Received {0}", job.Message);
        Console.WriteLine(" [>] Received {0}", job.Type);

        Thread.Sleep(job.HowManySecondsWillJobTake * 1000);
        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(" [>] Done");
        Console.ForegroundColor = ConsoleColor.Gray;

    };


    channel.BasicConsume(queue: "RabbitMqDotNet6Tutorial.02", autoAck: false,
        consumer: consumer);

    Console.WriteLine(" Press [enter] to end program.");
    Console.ReadLine();
}



ConsoleColor GetRandomConsoleColor()
{
    Random _random = new Random();
    var consoleColors = Enum.GetValues(typeof(ConsoleColor));
    return (ConsoleColor)consoleColors.GetValue(_random.Next(consoleColors.Length));
}




