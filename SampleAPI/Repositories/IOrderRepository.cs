using SampleAPI.Entities;
using SampleAPI.Requests;

namespace SampleAPI.Repositories
{
    public interface IOrderRepository
    {

        // TODO: Create repository methods.

        // Suggestions for repo methods:
        // public GetRecentOrders();
        // public AddNewOrder();
        Task<IEnumerable<Order>> GetRecentOrders();
        Task AddNewOrder(Order order);
        Task<IEnumerable<Order>> GetOrdersFromDate(DateTime startDate);
    }
}
