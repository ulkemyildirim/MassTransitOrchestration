using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared;
using System;
using System.Threading.Tasks;

namespace Order.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {

        readonly ApplicationDbContext _applicationDbContext;
        readonly ISendEndpointProvider _sendEndpointProvider;

        public OrdersController(ApplicationDbContext applicationDbContext
            , ISendEndpointProvider sendEndpointProvider
            )
        {
            _applicationDbContext = applicationDbContext;
            _sendEndpointProvider = sendEndpointProvider;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(OrderVM model)
        {
            Shared.Order order = new()
            {
                BuyerId = model.BuyerId,
                OrderItems = new System.Collections.Generic.List<OrderItem>() { new OrderItem
                {
                    Count = 1,
                    Price = 1,
                    ProductId = 1
                } },
                OrderStatus = Shared.OrderStatus.Suspend,
                TotalPrice = 10,
                CreatedDate = DateTime.Now
            };

            await _applicationDbContext.AddAsync<Shared.Order>(order);

            await _applicationDbContext.SaveChangesAsync();

            OrderStartedEvent orderStartedEvent = new()
            {
                BuyerId = model.BuyerId,
                OrderId = order.Id,
                TotalPrice = 10,
                OrderItems = new System.Collections.Generic.List<OrderItemMessage>(){ new Shared.OrderItemMessage
                {
                    Price = 1,
                    Count = 2,
                    ProductId = 3                } }
            };

            ISendEndpoint sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new($"queue:{RabbitMQSettings.StateMachine}"));
            await sendEndpoint.Send<OrderStartedEvent>(orderStartedEvent);

            return Ok(true);
        }
    }
}
