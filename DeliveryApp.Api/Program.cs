using CSharpFunctionalExtensions;
using DeliveryApp.Api;
using DeliveryApp.Core.Application.UseCases.Commands.AssignOrderToCourier;
using DeliveryApp.Core.Application.UseCases.Commands.CreateOrder;
using DeliveryApp.Core.Application.UseCases.Commands.MoveCourier;
using DeliveryApp.Core.Application.UseCases.Queries.GetBusyCouriers;
using DeliveryApp.Core.Application.UseCases.Queries.GetNotCompletedOrders;
using DeliveryApp.Core.Domain.Services;
using DeliveryApp.Core.Ports;
using DeliveryApp.Infrastructure.Adapters.Postgres;
using DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Primitives;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Health Checks
builder.Services.AddHealthChecks();

// Cors
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.AllowAnyOrigin(); // Не делайте так в проде!
        });
});

// Configuration
builder.Services.ConfigureOptions<SettingsSetup>();
var connectionString = builder.Configuration["CONNECTION_STRING"];
builder.Services.AddScoped<IDispatchService, DispatchService>();

// БД, ORM 
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(connectionString,
        sqlOptions => { sqlOptions.MigrationsAssembly("DeliveryApp.Infrastructure"); });
    options.EnableSensitiveDataLogging();
}
);

// UnitOfWork
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Repositories
builder.Services.AddScoped<ICourierRepository, CourierRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

// Mediator
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

// Mediator
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

// Commands
builder.Services.AddScoped<IRequestHandler<CreateOrderCommand, UnitResult<Error>>, CreateOrderCommandHandler>();
builder.Services.AddScoped<IRequestHandler<MoveCourierCommand, UnitResult<Error>>, MoveCourierCommandHandler>();
builder.Services.AddScoped<IRequestHandler<AssignOrderToCourierCommand, UnitResult<Error>>, AssignOrderToCourierCommandHandler>();

// Queries
builder.Services.AddScoped<IRequestHandler<GetBusyCouriersQuery, Maybe<GetBusyCouriersResponse>>>(serviceProvider =>
{
    var repository = serviceProvider.GetRequiredService<ICourierRepository>();
    return new GetBusyCouriersQueryHandler(repository);
});
builder.Services.AddScoped<IRequestHandler<GetNotCompletedOrdersQuery, Maybe<GetNotCompletedOrdersResponse>>>(_ =>
    new GetNotCompletedOrdersQueryHandler(connectionString));

var app = builder.Build();

// -----------------------------------
// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();
else
    app.UseHsts();

app.UseHealthChecks("/health");
app.UseRouting();

// Apply Migrations
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

app.Run();