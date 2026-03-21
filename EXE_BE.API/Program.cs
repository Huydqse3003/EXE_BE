using EXE_BE.Application;
using EXE_BE.Application.IServices;
using EXE_BE.Application.Services;
using EXE_BE.API.Hubs;
using EXE_BE.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 🔥 FIX PORT (đặt trước Build)
var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";
builder.WebHost.UseUrls($"http://*:{port}");

// DB
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

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
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 🔥 LUÔN bật Swagger
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.MapHub<RealtimeHub>("/hubs/realtime");

app.Run();