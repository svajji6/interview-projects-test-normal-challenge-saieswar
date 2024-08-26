using Microsoft.EntityFrameworkCore;
using SampleAPI.Entities;
using SampleAPI.Repositories;
using SampleAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<SampleApiDbContext>(options => options.UseInMemoryDatabase(databaseName: "SampleDB"));
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

SeedData(app);
app.Run();



void SeedData(IApplicationBuilder app)
{
    using var scope = app.ApplicationServices.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<SampleApiDbContext>();

    if (context.Orders.Any())
    {
        return; 
    }

    context.Orders.AddRange(new Order
    {
        Id = Guid.NewGuid(),
        EntryDate = DateTime.UtcNow.AddHours(-1),
        Name = "Saieswar orders",
        Description = "This is a Saieswar orders",
        IsInvoiced = true,
        IsDeleted = false
    },
    new Order
    {
        Id = Guid.NewGuid(),
        EntryDate = DateTime.UtcNow.AddHours(-1),
        Name = "Vajji orders",
        Description = "This is the Vajji order",
        IsInvoiced = true,
        IsDeleted = false
    });

    context.SaveChanges();
}
