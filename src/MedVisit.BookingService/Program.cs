using System.Text;
using MedVisit.BookingService;
using MedVisit.BookingService.RabbitMq;
using MedVisit.BookingService.Services;
using MedVisit.Core.Middleware;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

var host = builder.Configuration["PostgresConnection:Host"];
var port = builder.Configuration["PostgresConnection:Port"];
var database = builder.Configuration["PostgresConnection:Database"];
var username = Environment.GetEnvironmentVariable("DB_USERNAME");
var password = Environment.GetEnvironmentVariable("DB_PASSWORD");

var connectionString = $"Host={host};Port={port};Database={database};Username={username};Password={password}";

builder.Services.AddDbContext<BookingDbContext>(options =>
    options.UseNpgsql(connectionString));
// Add services to the container.

builder.Services.AddScoped<IOrderService, OrderService>(); 
builder.Services.AddScoped<ISagaStepsService, SagaStepsService>();
builder.Services.AddSingleton<EventPublisher>();
builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(builder.Configuration["Redis:Host"])
);

builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthorization();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient("PaymentService", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Endpoints:PaymentService"]);
});
builder.Services.AddHttpClient("ScheduleService", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Endpoints:ScheduleService"]);
});


var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<BookingDbContext>();
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<AuthMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
