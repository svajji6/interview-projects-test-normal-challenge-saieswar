using Microsoft.EntityFrameworkCore;
using SampleAPI.Entities;
using SampleAPI.Repositories;
using SampleAPI.Requests;
using SampleAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleAPI.Tests.Services
{
    public class OrderServicesTests
    {
        [Fact]
        public async Task GetRecentOrders_ShouldReturnRecentOrdersFromRepository()
        {
            // Arrange

            using (var context = MockSampleApiDbContextFactory.GenerateMockContext())
            {
                context.Orders.AddRange(
                    new Order { Id = Guid.NewGuid(), EntryDate = DateTime.UtcNow, IsDeleted = false },
                    new Order { Id = Guid.NewGuid(), EntryDate = DateTime.UtcNow.AddHours(-2), IsDeleted = false },
                    new Order { Id = Guid.NewGuid(), EntryDate = DateTime.UtcNow.AddDays(-2), IsDeleted = false },
                    new Order { Id = Guid.NewGuid(), EntryDate = DateTime.UtcNow, IsDeleted = true }
                );
                await context.SaveChangesAsync();
            }

            using (var context = MockSampleApiDbContextFactory.GenerateMockContext())
            {
                var repository = new OrderRepository(context);
                var service = new OrderService(repository);

                // Act
                var recentOrders = await service.GetRecentOrders();

                // Assert
                Assert.Equal(2, recentOrders.Count());
                Assert.All(recentOrders, o => Assert.False(o.IsDeleted));
                Assert.All(recentOrders, o => Assert.True(o.EntryDate > DateTime.UtcNow.AddDays(-1)));
            }
        }
        [Fact]
        public async Task SubmitOrder_ShouldCreateAndReturnOrderResponse()
        {
            // Arrange
   
            using (var context = MockSampleApiDbContextFactory.GenerateMockContext())
            {
                var repository = new OrderRepository(context);
                var service = new OrderService(repository);

                var createOrderRequest = new CreateOrderRequest
                {
                    Name = "Test Order",
                    Description = "Test Description"
                };

                // Act
                var response = await service.SubmitOrder(createOrderRequest);

                // Assert
                var createdOrder = await context.Orders.FindAsync(response.Id);

                Assert.NotNull(createdOrder);
                Assert.Equal(response.Id, createdOrder.Id);
                Assert.Equal(response.Name, createdOrder.Name);
                Assert.Equal(response.Description, createdOrder.Description);
                Assert.False(response.IsDeleted);
                Assert.False(response.IsInvoiced);
            }
        }

    }
}
