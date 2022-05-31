using MassTransit;
using Shared;
using System.Threading.Tasks;

namespace Order.API
{
    public class OrderFailedEventConsumer : IConsumer<OrderFailedEvent>
    {
        readonly ApplicationDbContext _context;
        public OrderFailedEventConsumer(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task Consume(ConsumeContext<OrderFailedEvent> context)
        {
            Shared.Order order = await _context.FindAsync<Shared.Order>(context.Message.OrderId);
            if (order != null)
            {
                order.OrderStatus = OrderStatus.Fail;
                await _context.SaveChangesAsync();
                //Console.WriteLine(context.Message.Message);
            }
        }
    }
}
