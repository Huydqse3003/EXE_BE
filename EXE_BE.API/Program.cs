using EXE_BE.Application;
using EXE_BE.Application.IServices;
using EXE_BE.Application.Services;
using EXE_BE.API.Hubs;
using EXE_BE.API.Services;
using EXE_BE.Domain;
using EXE_BE.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var appSettings = builder.Configuration.Get<AppSettings>() ?? new AppSettings();
builder.Services.AddSingleton(appSettings);

var connectionString = appSettings.ConnectionStrings.DefaultConnection;
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IGameplayService, GameplayService>();
builder.Services.AddScoped<IUserHabitService, UserHabitService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IClaimService, ClaimService>();
builder.Services.AddScoped<IEmailService, SmtpEmailService>();
builder.Services.AddScoped<IFocusMusicService, FocusMusicService>();
builder.Services.AddScoped<IFriendshipService, FriendshipService>();
builder.Services.AddScoped<ISubscriptionPackageService, SubscriptionPackageService>();
builder.Services.AddScoped<IUserSubscriptionService, UserSubscriptionService>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient<IPaymentService, PayOSPaymentService>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = !string.IsNullOrWhiteSpace(appSettings.JwtSettings.Issuer),
            ValidateAudience = !string.IsNullOrWhiteSpace(appSettings.JwtSettings.Audience),
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = appSettings.JwtSettings.Issuer,
            ValidAudience = appSettings.JwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.JwtSettings.Secret)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers();
builder.Services.AddSignalR();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "EXE_BE API", Version = "v1" });

    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Nhập: Bearer {token}",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    c.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<RealtimeHub>("/hubs/realtime");

app.Run();
