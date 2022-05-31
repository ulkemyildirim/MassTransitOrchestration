using MassTransit;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockConsole
{
    public class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>
    {       

        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            Console.WriteLine("CorrelationId" + context.Message.CorrelationId);

            //Eğer fazlaysa sipariş edilen ürünlerin stok miktarı güncelleniyor.
            ISendEndpoint sendEndpoint = await context.GetSendEndpoint(new Uri($"queue:{RabbitMQSettings.StateMachine}"));

            if (context.Message.OrderItems.Count == 2)
            {
                StockReservedEvent stockReservedEvent = new(context.Message.CorrelationId)
                {
                    OrderItems = context.Message.OrderItems
                };
                await sendEndpoint.Send(stockReservedEvent);
            }
            //Eğer az ise siparişin iptal edilmesi için gerekli event gönderiliyor.
            else
            {
                StockNotReservedEvent stockNotReservedEvent = new(context.Message.CorrelationId)
                {
                    Message = "Stok yetersiz..."
                };

                await sendEndpoint.Send(stockNotReservedEvent);
            }
        }
    }
}
