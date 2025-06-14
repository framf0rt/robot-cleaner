using Microsoft.EntityFrameworkCore;
using RobotCleaner;

var builder = WebApplication.CreateBuilder(args);

var config = new ConfigurationBuilder()
    .AddJsonFile("secrets.json", false)
    .AddEnvironmentVariables()
    .Build();

var connectionString = config.GetConnectionString("Postgres");

builder.Services.AddDbContextPool<ExecutionDbContext>(options => options.UseNpgsql(connectionString
));
builder.Services.AddSingleton<ExecutionDbContext>();
builder.Services.AddControllers();

// Config json d

var app = builder.Build();
app.MapControllers();

app.Run();