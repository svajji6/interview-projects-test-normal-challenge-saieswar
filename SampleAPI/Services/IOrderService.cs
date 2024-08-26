using SampleAPI.Entities;
using SampleAPI.Requests;

namespace SampleAPI.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetRecentOrders();
        Task<CreateOrderResponse> SubmitOrder(CreateOrderRequest CreateOrderRequest);
        Task<IEnumerable<Order>> GetOrdersExcludingHolidaysAndWeekends(int days);

    }
}
