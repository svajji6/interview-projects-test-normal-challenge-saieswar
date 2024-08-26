using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using SampleAPI.Entities;
using SampleAPI.Repositories;
using SampleAPI.Requests;
using System;

namespace SampleAPI.Tests.Repositories
{

    public class OrderRepositoryTests
    {

        [Fact]
        public async Task GetRecentOrders_ShouldReturnRecentOrders()
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

                // Act
                var recentOrders = await repository.GetRecentOrders();

                // Assert
                Assert.Equal(2, recentOrders.Count());
                Assert.All(recentOrders, o => Assert.False(o.IsDeleted));
                Assert.All(recentOrders, o => Assert.True(o.EntryDate > DateTime.UtcNow.AddDays(-1)));
            }
        }

        [Fact]
        public async Task AddNewOrder_ShouldAddOrder()
        {
            // Arrange
            var order = new Order { Id = Guid.NewGuid(), EntryDate = DateTime.UtcNow, IsDeleted = false };

            using (var context = MockSampleApiDbContextFactory.GenerateMockContext())
            {
                var repository = new OrderRepository(context);

                // Act
                await repository.AddNewOrder(order);
                await context.SaveChangesAsync();
            }

            using (var context = MockSampleApiDbContextFactory.GenerateMockContext())
            {
                var addedOrder = await context.Orders.FindAsync(order.Id);

                // Assert
                Assert.NotNull(addedOrder);
                Assert.Equal(order.Id, addedOrder.Id);
            }
        }

    }
}
  