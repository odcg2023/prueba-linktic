using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using ProductosService.Service.HealthChecks;
using ProductosService.Service.Middleware;
using SeguridadService.Application.Interfaces;
using SeguridadService.Application.Services;
using SeguridadService.Domain.Interfaces.Repository;
using SeguridadService.Infraestructure.Context;
using SeguridadService.Infraestructure.Repository;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .MinimumLevel.Debug()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File(
            path: "C:\\TempLogs\\ServicioSeguridadLog-.json",
            rollingInterval: RollingInterval.Day,
            formatter: new Serilog.Formatting.Compact.RenderedCompactJsonFormatter());
});


builder.Services.AddDbContext<ContextSeguridad>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DbPruebaTecnica")).EnableSensitiveDataLogging());

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
builder.Services.AddScoped<DbContext, ContextSeguridad>();
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<ILoginService, LoginService>();

builder.Services.AddAuthorization(); 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
