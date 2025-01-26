using MedVisit.Core.Middleware;
using MedVisit.NotificationService;
using MedVisit.NotificationService.RabbitMq;
using MedVisit.NotificationService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var host = builder.Configuration["PostgresConnection:Host"];
var port = builder.Configuration["PostgresConnection:Port"];
var database = builder.Configuration["PostgresConnection:Database"];
var username = Environment.GetEnvironmentVariable("DB_USERNAME");
var password = Environment.GetEnvironmentVariable("DB_PASSWORD");

var connectionString = $"Host={host};Port={port};Database={database};Username={username};Password={password}";

builder.Services.AddDbContext<NotificationDbContext>(options =>
    options.UseNpgsql(connectionString)); 
builder.Services.AddScoped<INotificationService, NotificationService>();

builder.Services.AddHostedService<NotificationListenerService>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthorization();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();
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
