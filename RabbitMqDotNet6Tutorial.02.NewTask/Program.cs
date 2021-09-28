﻿using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory() { HostName = "localhost" };
using (var connection = factory.CreateConnection())

using (var channel = connection.CreateModel())
{
    channel.QueueDeclare(queue: "RabbitMqDotNet6Tutorial.02", true,
        false, false, null);

    while (true)
    {
        Console.WriteLine("Write [.] to increase time to do job for worker");
        Console.WriteLine("Write [q] or [Q] to exit.");

        string whatuserwrote = Console.ReadLine();

        if (whatuserwrote == "q" || whatuserwrote == "Q")
            break;
        if (string.IsNullOrEmpty(whatuserwrote))
            continue;

        Job job = CreateJob(whatuserwrote);
        string message = JsonConvert.SerializeObject(job);

        var body = Encoding.UTF8.GetBytes(message);

        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;

        channel.BasicPublish("", routingKey: "RabbitMqDotNet6Tutorial.02",
            properties, body);

        WriteMessageOnConsole(message);
    }

}


void WriteMessageOnConsole(string message)
{
    Console.WriteLine("");
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("\tSent {0}", message);
    Console.ForegroundColor = ConsoleColor.Gray;
    Console.WriteLine("");
}

Job CreateJob(string usertext)
{ 
    int howManySecondsWillJobTake = usertext.Split('.').Length - 1;

    string message = usertext.Replace(",", "")
        .Replace(".", "");

    return new Job()
    {
        Message = message,
        HowManySecondsWillJobTake = howManySecondsWillJobTake,
        Type = JobType.SendEmail
    };
}