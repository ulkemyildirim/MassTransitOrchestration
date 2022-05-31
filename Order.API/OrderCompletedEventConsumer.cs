using MassTransit;
using Shared;
using System.Threading.Tasks;

namespace Order.API
{
    public class OrderCompletedEventConsumer : IConsumer<OrderCompletedEvent>
    {
        readonly ApplicationDbContext _applicationDbContext;
        public OrderCompletedEventConsumer(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }
        public async Task Consume(ConsumeContext<OrderCompletedEvent> context)
        {
            Shared.Order order = await _applicationDbContext.Orders.FindAsync(context.Message.OrderId);
            if (order != null)
            {
                order.OrderStatus = OrderStatus.Completed;
                await _applicationDbContext.SaveChangesAsync();
            }
        }
    }
}
