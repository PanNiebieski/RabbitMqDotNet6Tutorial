using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.ComponentModel;
using System.Reflection;
using System.Text;

var factory = new ConnectionFactory() { HostName = "localhost" };
using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
    channel.ExchangeDeclare(exchange: "RabbitMqDotNet6Tutorial.04", type: ExchangeType.Direct);
    var queueName = channel.QueueDeclare().QueueName;

    RoutingKey key1 = RoutingKey.Mail;
    channel.QueueBind(queue: queueName, exchange: "RabbitMqDotNet6Tutorial.04",
        routingKey: key1.GetDescription<RoutingKey>());

    RoutingKey key2 = RoutingKey.Test;
    channel.QueueBind(queue: queueName, exchange: "RabbitMqDotNet6Tutorial.04",
        routingKey: key2.GetDescription<RoutingKey>());

    Console.WriteLine(" [*] Waiting for messages.");

    var consumer = new EventingBasicConsumer(channel);
    consumer.Received += (model, ea) =>
    {
        var message = Encoding.UTF8.GetString(ea.Body.ToArray());
        var routingKey = ea.RoutingKey;
        Console.WriteLine(" [x] Received '{0}':'{1}'", routingKey, message);
    };
    
    channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

    Console.WriteLine(" Press [enter] to exit.");
    Console.ReadLine();
}



public enum RoutingKey
{
    [Description("Email")]
    Email = 1,
    [Description("Mail")]
    Mail = 2,
    [Description("Test")]
    Test = 3
}

public static class Helper
{
    public static string GetDescription<T>(this T enumerationValue)
    where T : struct
    {
        Type type = enumerationValue.GetType();
        if (!type.IsEnum)
        {
            throw new ArgumentException
                ("EnumerationValue must be of Enum type", "enumerationValue");
        }

        //Tries to find a DescriptionAttribute for a potential friendly name
        //for the enum
        MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString());
        if (memberInfo != null && memberInfo.Length > 0)
        {
            object[] attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attrs != null && attrs.Length > 0)
            {
                //Pull out the description value
                return ((DescriptionAttribute)attrs[0]).Description;
            }
        }
        //If we have no description attribute, just return the ToString of the enum
        return enumerationValue.ToString();
    }
}