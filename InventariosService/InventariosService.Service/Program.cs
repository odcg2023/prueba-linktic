﻿using InventariosService.Application.Helpers;
using InventariosService.Application.Interfaces;
using InventariosService.Application.Interfaces.Integrations;
using InventariosService.Application.Services;
using InventariosService.Common;
using InventariosService.Domain.Interfaces.Repository;
using InventariosService.Infraestructure.Context;
using InventariosService.Infraestructure.Integrations.Services;
using InventariosService.Infraestructure.Repository;
using InventariosService.Service.HealthChecks;
using InventariosService.Service.Helpers;
using InventariosService.Service.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Polly;
using Serilog;
using Serilog.Events;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .MinimumLevel.Debug()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File(
            path: Path.Combine(Directory.GetCurrentDirectory(), "Logs", "log-.json"),
            rollingInterval: RollingInterval.Day,
            formatter: new Serilog.Formatting.Compact.RenderedCompactJsonFormatter());
});


builder.Services.AddDbContext<ContextInventarios>(options =>
    options.UseSqlServer(builder.Configuration.GetSecureConnectionString("DbPruebaTecnica")).EnableSensitiveDataLogging());

builder.Services.AddHttpContextAccessor();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddAppHealthChecks(builder.Configuration);
builder.Services.Configure<HealthCheckPublisherOptions>(options =>
{
    options.Delay = TimeSpan.FromSeconds(2); 
    options.Period = TimeSpan.FromSeconds(30); 
    options.Predicate = _ => true; 
    options.Timeout = TimeSpan.FromSeconds(10);
});
builder.Services.AddSingleton<IHealthCheckPublisher, LoggingHealthCheckPublisher>();

builder.Services.AddScoped(typeof(UnitOfWork));
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<DbContext, ContextInventarios>();
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<IInventarioService, InventarioService>();
builder.Services.AddScoped<ICryptoHelper, CryptoHelper>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddHttpClient<IProductoApiService, ProductoApiService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["UrlApiProductos"]); 
})
.AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(300)))
.AddTransientHttpErrorPolicy(p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Authentication:Issuer"],
            ValidAudience = builder.Configuration["Authentication:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(Crypto.Decrypt(AppConstants.CryptoKeys.KeyToken)))
        };
    });

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "InventariosService.Service", Version = "v1" });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    // Definir el esquema de seguridad JWT
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingrese el token JWT usando el esquema Bearer. Ejemplo: \"Bearer {token}\""
    });

    // Agregar el requisito global de seguridad
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseAppHealthCheckEndpoints();
app.Run();
Log.CloseAndFlush();
