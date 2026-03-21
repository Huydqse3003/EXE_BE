using EXE_BE.Application;
using EXE_BE.Application.IServices;
using EXE_BE.Application.Services;
using EXE_BE.API.Hubs;
using EXE_BE.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// PORT
var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";
builder.WebHost.UseUrls($"http://*:{port}");

// DB
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine("DB_STRING: " + connectionString);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 32))));

// Services
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IGameplayService, GameplayService>();
builder.Services.AddScoped<IUserHabitService, UserHabitService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IFocusMusicService, FocusMusicService>();
builder.Services.AddScoped<IFriendshipService, FriendshipService>();

builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();

// Swagger + JWT
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter JWT token"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

// Swagger luôn bật
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.MapHub<RealtimeHub>("/hubs/realtime");

// 🔥 CHECK DB CONNECTION
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        logger.LogInformation("🔄 Checking database connection...");

        var canConnect = db.Database.CanConnect();

        if (canConnect)
        {
            logger.LogInformation("✅ DATABASE CONNECTED SUCCESSFULLY");
        }
        else
        {
            logger.LogError("❌ DATABASE CONNECTION FAILED (CanConnect = false)");
        }
    }
    catch (Exception ex)
    {
        logger.LogError("💥 DATABASE CONNECTION ERROR: {Message}", ex.Message);
        logger.LogError("💥 STACK TRACE: {StackTrace}", ex.StackTrace);
    }
}

app.Run();