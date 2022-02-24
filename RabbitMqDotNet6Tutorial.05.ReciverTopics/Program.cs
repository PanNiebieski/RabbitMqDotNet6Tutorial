﻿using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var factory = new ConnectionFactory() { HostName = "localhost" };
using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
    channel.ExchangeDeclare(exchange: "RabbitMqDotNet6Tutorial.05", type: ExchangeType.Topic);
    var queueName = channel.QueueDeclare().QueueName;

    if (args.Length < 1)
    {
        Console.Error.WriteLine("Usage: {0} [binding_key...]",
                                Environment.GetCommandLineArgs()[0]);
        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
        Environment.ExitCode = 1;
        return;
    }

    foreach (var bindingKey in args)
    {
        channel.QueueBind(queue: queueName,
                          exchange: "RabbitMqDotNet6Tutorial.05",
                          routingKey: bindingKey);
    }

    Console.WriteLine(" [*] Waiting for messages. To exit press CTRL+C");

    var consumer = new EventingBasicConsumer(channel);
    consumer.Received += (model, ea) =>
    {
        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        var routingKey = ea.RoutingKey;
        Console.WriteLine(" [x] Received '{0}':'{1}'",
                          routingKey,
                          message);
    };
    channel.BasicConsume(queue: queueName,
                         autoAck: true,
                         consumer: consumer);

    Console.WriteLine(" Press [enter] to exit.");
    Console.ReadLine();
}