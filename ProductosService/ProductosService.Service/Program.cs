using Microsoft.EntityFrameworkCore;
using ProductosService.Application.Interfaces;
using ProductosService.Application.Services;
using ProductosService.Domain.Interfaces.Repository;
using ProductosService.Infraestructure.Context;
using ProductosService.Infraestructure.Repository;
using ProductosService.Service.Middleware;
using Serilog;
using Serilog.Events;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .MinimumLevel.Debug()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File(
            path: "C:\\TempLogs\\log-.json",
            rollingInterval: RollingInterval.Day,
            formatter: new Serilog.Formatting.Compact.RenderedCompactJsonFormatter());
});


builder.Services.AddDbContext<ContextProductos>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DbPruebaTecnica")).EnableSensitiveDataLogging());

builder.Services.AddHttpContextAccessor();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped(typeof(UnitOfWork));
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<DbContext, ContextProductos>();
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<IProductoService, ProductoService>();

//builder.Services.AddAuthentication("Bearer")
//    .AddJwtBearer("Bearer", options =>
//    {
//        options.Authority = "https://localhost:5001"; // actualiza esto según tu microservicio de login
//        options.RequireHttpsMetadata = false;
//        options.Audience = "productosapi"; // nombre de audiencia esperada
//    });

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
Log.Information("🚀 Serilog está funcionando correctamente en {Time}", DateTime.UtcNow);
app.Run();
Log.CloseAndFlush();
