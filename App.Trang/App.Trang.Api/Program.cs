using App.Trang.Api.Common.Behaviors;
using App.Trang.Api.Data;
using App.Trang.Api.Features.Categories;
using App.Trang.Api.Features.Customers;
using App.Trang.Api.Features.Orders;
using App.Trang.Api.Features.Products;
using App.Trang.Api.Features.Providers;
using App.Trang.Api.Features.WareHouses;
using App.Trang.Api.Endpoints;
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
        policy.SetIsOriginAllowed(x=> true)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// ===== Seed dữ liệu ban đầu =====
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync(); // Tự động tạo bảng nếu chưa có
    await DbSeeder.SeedAsync(db);
}


// ===== Middleware =====
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Web Bán Hàng Trang API v1");
});
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("AllowAngular");

// ===== Global Exception Handler =====
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

        var errorMessage = string.Join("\n", ex.Errors.Select(e => e.ErrorMessage));
        var result = new App.Trang.Api.Common.Models.ApiResponse(false, errorMessage);

        await context.Response.WriteAsJsonAsync(result);
    }
    catch (Exception ex)
    {
        context.Response.StatusCode = 400;
        context.Response.ContentType = "application/json";

        var result = new App.Trang.Api.Common.Models.ApiResponse(false, ex.Message);

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
