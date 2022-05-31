using System;
using Shared;
using MassTransit;

namespace StockConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var bus = Bus.Factory.CreateUsingRabbitMq(factory =>
            {

                factory.Host(Shared.RabbitMqConsts.RabbitMqUri, configurator =>
                {
                    configurator.Username(Shared.RabbitMqConsts.UserName);
                    configurator.Password(Shared.RabbitMqConsts.Password);
                });

                factory.ReceiveEndpoint(RabbitMQSettings.Stock_OrderCreatedEventQueue, endpoint => endpoint.Consumer<OrderCreatedEventConsumer>());
            });

            bus.Start();
            Console.ReadLine();
            bus.Stop();
        }
    }
}
