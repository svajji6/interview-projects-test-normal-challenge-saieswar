using SampleAPI.Entities;
using SampleAPI.Repositories;
using SampleAPI.Requests;

namespace SampleAPI.Services
{
    public class OrderService: IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }
        public async Task<IEnumerable<Order>> GetRecentOrders()
        {
            return await _orderRepository.GetRecentOrders();
        }
        public async Task<CreateOrderResponse> SubmitOrder(CreateOrderRequest createOrder)
        {
            var order = new Order
            {
                Id = Guid.NewGuid(),
                EntryDate = DateTime.UtcNow,
                Name = createOrder.Name,
                Description = createOrder.Description,
            };
            await _orderRepository.AddNewOrder(order);
            var createorderResponse = new CreateOrderResponse
            {
                Id=order.Id,
                EntryDate = order.EntryDate,
                Name = order.Name,
                Description=order.Description,
                IsInvoiced = order.IsInvoiced,
                IsDeleted = order.IsDeleted,
            };

            return createorderResponse;
        }

        public async Task<IEnumerable<Order>> GetOrdersExcludingHolidaysAndWeekends(int days)
        {
            var targetDate = CalculateTargetDate(days);
            return await _orderRepository.GetOrdersFromDate(targetDate);
        }

        private DateTime CalculateTargetDate(int days)
        {
            DateTime currentDate = DateTime.UtcNow;
            int daysAdded = 0;

            while (daysAdded < days)
            {
                currentDate = currentDate.AddDays(-1);

                if (!IsWeekend(currentDate) && !IsHoliday(currentDate))
                {
                    daysAdded++;
                }
            }

            return currentDate;
        }

        private bool IsWeekend(DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
        }

        private bool IsHoliday(DateTime date)
        {
            var holidays = GetHolidays(date.Year);
            return holidays.Contains(date.Date);
        }

        //  i have added statical data for holiday dates
        private List<DateTime> GetHolidays(int year)
        {
            return new List<DateTime>
            {
                new DateTime(year, 1, 1),  
                new DateTime(year, 8, 15),  
                new DateTime(year, 12, 25), 
            };
        }


    }
}
