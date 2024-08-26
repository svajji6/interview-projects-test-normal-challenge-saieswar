using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SampleAPI.Controllers;
using SampleAPI.Entities;
using SampleAPI.Repositories;
using SampleAPI.Requests;
using SampleAPI.Services;

namespace SampleAPI.Tests.Controllers
{
    public class OrdersControllerTests
    {
        private readonly OrdersController _controller;
        private readonly Mock<IOrderService> _mockOrderService;
        private readonly Mock<ILogger<OrdersController>> _mockLogger;

        public OrdersControllerTests()
        {
            _mockOrderService = new Mock<IOrderService>();
            _mockLogger = new Mock<ILogger<OrdersController>>();
            _controller = new OrdersController(_mockOrderService.Object, _mockLogger.Object);
        }
        [Fact]
        public async Task GetOrders_ShouldReturnOkWithRecentOrders()
        {
            // Arrange
                    var orders = new List<Order>
            {
                new Order { Id = Guid.NewGuid(), Name = "Order1", Description = "Description1", EntryDate = DateTime.UtcNow },
                new Order { Id = Guid.NewGuid(), Name = "Order2", Description = "Description2", EntryDate = DateTime.UtcNow.AddHours(-2) }
            };

            _mockOrderService.Setup(s => s.GetRecentOrders()).ReturnsAsync(orders);

            // Act
            var result = await _controller.GetOrders();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedOrders = Assert.IsType<List<Order>>(okResult.Value);
            Assert.Equal(2, returnedOrders.Count);
        }

   
        [Fact]
        public async Task GetOrders_ShouldReturnInternalServerErrorOnException()
        {
            // Arrange
            _mockOrderService.Setup(s => s.GetRecentOrders()).ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.GetOrders();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("Internal server error", statusCodeResult.Value);
            _mockLogger.Verify(l => l.LogError(It.IsAny<Exception>(), "Error retrieving recent orders"), Times.Once);
        }
        [Fact]
        public async Task CreateOrder_ShouldReturnOkWithCreatedOrder()
        {
            // Arrange
            var orderRequest = new CreateOrderRequest { Name = "Order1", Description = "Description1" };
            var orderResponse = new CreateOrderResponse
            {
                Id = Guid.NewGuid(),
                Name = "Order1",
                Description = "Description1",
                EntryDate = DateTime.UtcNow
            };

            _mockOrderService.Setup(s => s.SubmitOrder(orderRequest)).ReturnsAsync(orderResponse);

            // Act
            var result = await _controller.CreateOrder(orderRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedOrder = Assert.IsType<CreateOrderResponse>(okResult.Value);
            Assert.Equal(orderResponse.Id, returnedOrder.Id);
            Assert.Equal(orderResponse.Name, returnedOrder.Name);
        }

        [Fact]
        public async Task CreateOrder_ShouldReturnBadRequestForInvalidModel()
        {
            // Arrange
            _controller.ModelState.AddModelError("Name", "Required");

            var orderRequest = new CreateOrderRequest();

            // Act
            var result = await _controller.CreateOrder(orderRequest);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }

        [Fact]
        public async Task CreateOrder_ShouldReturnInternalServerErrorOnException()
        {
            // Arrange
            var orderRequest = new CreateOrderRequest { Name = "Clothes", Description = "Description1" };

            _mockOrderService.Setup(s => s.SubmitOrder(orderRequest)).ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.CreateOrder(orderRequest);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("Internal server error", statusCodeResult.Value);
            _mockLogger.Verify(l => l.LogError(It.IsAny<Exception>(), "Error submitting order"), Times.Once);
        }
        [Fact]
        public async Task GetOrdersExcludingHolidaysAndWeekends_ShouldReturnOkWithOrders()
        {
            // Arrange
            var days = 5;
            var orders = new List<Order>
            {
                new Order { Id = Guid.NewGuid(), Name = "Order 1", EntryDate = DateTime.UtcNow.AddDays(-2) },
                new Order { Id = Guid.NewGuid(), Name = "Order 2", EntryDate = DateTime.UtcNow.AddDays(-3) }
            };

            _mockOrderService.Setup(service => service.GetOrdersExcludingHolidaysAndWeekends(days))
                .ReturnsAsync(orders);

            // Act
            var result = await _controller.GetOrdersExcludingHolidaysAndWeekends(days) as OkObjectResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);
            result.Value.Should().BeEquivalentTo(orders);
            _mockOrderService.Verify(service => service.GetOrdersExcludingHolidaysAndWeekends(days), Times.Once);
        }

        [Fact]
        public async Task GetOrdersExcludingHolidaysAndWeekends_WithInvalidDays_ShouldReturnBadRequest()
        {
            // Arrange
            var days = -1;

            // Act
            var result = await _controller.GetOrdersExcludingHolidaysAndWeekends(days) as BadRequestObjectResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(400);
            result.Value.Should().Be("Days must be greater than 0.");
        }

        [Fact]
        public async Task GetOrdersExcludingHolidaysAndWeekends_ShouldReturnStatusCode500OnException()
        {
            // Arrange
            var days = 5;
            _mockOrderService.Setup(service => service.GetOrdersExcludingHolidaysAndWeekends(days))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.GetOrdersExcludingHolidaysAndWeekends(days) as StatusCodeResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(500);
            _mockLogger.Verify(logger => logger.LogError(It.IsAny<Exception>(), "Error fetching orders"), Times.Once);
        }
    }
}

