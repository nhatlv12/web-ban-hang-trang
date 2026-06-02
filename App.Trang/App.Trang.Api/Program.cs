using App.Trang.Api.Common.Behaviors;
using App.Trang.Api.Data;
using App.Trang.Api.Features.Categories;
using App.Trang.Api.Features.Customers;
using App.Trang.Api.Features.Orders;
using App.Trang.Api.Features.Products;
using App.Trang.Api.Features.Providers;
using App.Trang.Api.Features.WareHouses;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ===== Database =====
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ===== MediatR + CQRS =====
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.AddBehavior(typeof(MediatR.IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
});

// ===== FluentValidation =====
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

// ===== Swagger / OpenAPI =====
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Web Bán Hàng Trang API", Version = "v1" });
});

// ===== CORS (cho Angular dev server) =====
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// ===== Middleware =====
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Web Bán Hàng Trang API v1");
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAngular");

// ===== Global Exception Handler cho Validation =====
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (ValidationException ex)
    {
        context.Response.StatusCode = 400;
        context.Response.ContentType = "application/json";

        var errors = ex.Errors.Select(e => e.ErrorMessage).ToList();
        var result = new
        {
            Success = false,
            Message = "Validation failed",
            Errors = errors
        };

        await context.Response.WriteAsJsonAsync(result);
    }
});

// ===== Map Minimal API Endpoints =====
app.MapProviderEndpoints();
app.MapCustomerEndpoints();
app.MapCategoryEndpoints();
app.MapProductEndpoints();
app.MapWareHouseEndpoints();
app.MapOrderEndpoints();

app.Run();
