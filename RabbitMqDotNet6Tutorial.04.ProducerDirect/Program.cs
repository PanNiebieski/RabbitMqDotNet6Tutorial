using RabbitMQ.Client;
using System.ComponentModel;
using System.Reflection;
using System.Text;

var factory = new ConnectionFactory() { HostName = "localhost" };
using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{

    channel.ExchangeDeclare(exchange: "RabbitMqDotNet6Tutorial.04", type: ExchangeType.Direct);

    while (true)
    {
        Console.WriteLine("Write what you want to send");
        Console.WriteLine("Write nothing to exit.");

        string usermessage = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(usermessage))
            continue; 

        Console.WriteLine("Choose your routingKey");
        Console.WriteLine("1. Email");
        Console.WriteLine("2. Mail");
        Console.WriteLine("3. Test");

        char userRoutingKey = Console.ReadKey().KeyChar;
        int userRoutingNumber = int.Parse(userRoutingKey.ToString());

        if (userRoutingNumber == 1 || userRoutingNumber == 2 || userRoutingNumber == 3)
        {
            RoutingKey key = (RoutingKey)int.Parse(userRoutingKey.ToString());

            var body = Encoding.UTF8.GetBytes(usermessage);

            channel.BasicPublish(exchange: "RabbitMqDotNet6Tutorial.04",
                routingKey: key.GetDescription<RoutingKey>(),
                basicProperties: null, body: body);

            WriteMessageOnConsole($"{usermessage}:{key.GetDescription<RoutingKey>()}");
     
        }

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


