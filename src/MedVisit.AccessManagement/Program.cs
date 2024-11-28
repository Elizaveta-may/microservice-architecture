using MedVisit.Common.AuthDbContext;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

if (builder.Environment.IsDevelopment())
{
    var connectionString = (builder.Configuration["PostgresConnection"]);

    builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseNpgsql(builder.Configuration["PostgresConnection"]));

}
else
{
    var host = builder.Configuration["PostgresConnection:Host"];
    var port = builder.Configuration["PostgresConnection:Port"];
    var database = builder.Configuration["PostgresConnection:Database"];
    var username = Environment.GetEnvironmentVariable("DB_USERNAME");
    var password = Environment.GetEnvironmentVariable("DB_PASSWORD");

    var connectionString = $"Host={host};Port={port};Database={database};Username={username};Password={password}";

    builder.Services.AddDbContext<AuthDbContext>(options =>
        options.UseNpgsql(connectionString));
}

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    dbContext.Database.Migrate(); 
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
