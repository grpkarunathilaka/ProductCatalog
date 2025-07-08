using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ProductCatalog.Api.Middleware;
using ProductCatalog.Application.Interfaces;
using ProductCatalog.Application.Services;
using ProductCatalog.Application.Validators;
using ProductCatalog.Domain.Models;
using ProductCatalog.Infrastructure.Data;
using ProductCatalog.Infrastructure.Repositories;
using Serilog;
using System.Reflection;
using FluentValidation.AspNetCore;


var builder = WebApplication.CreateBuilder(args);

//Configure Serilog for logging
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

//Add Serilog
builder.Host.UseSerilog();

//Add Services to the container
builder.Services.AddControllers();

//Configure Entity Framework with In-Memory Database
builder.Services.AddDbContext<ProductDbContext>(options => 
    options.UseInMemoryDatabase("ProductDatabase"));

//Register Repositories
builder.Services.AddScoped<IProductRepository, ProductRepository>();

//Register Services
builder.Services.AddScoped<IProductService, ProductService>();

//Add FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductDtoValidator>();

//Configure Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Version = "v1",
        Title = "Product Catelog API",
        Description = "A comprehensive Web API for managing a product catalog with CRUD operations, filtering, pagination and validation.",
        Contact = new()
        {
            Name = "Ranga",
            Email = ""
        }
    });

    //Include XML comments for better documentation
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if(File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

//Configure CORS for development
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCorsPolicy", builder =>
    {
        builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader(); 
    });
});

//Build the application
var app = builder.Build();

//Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Product Catalog API V1");
        c.RoutePrefix = string.Empty;
    });
    app.UseCors("DevCorsPolicy");
}

//Add custom middleware
app.UseMiddleware<AuditLoggingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseHttpsRedirection();

//Configure global exception handling
app.UseExceptionHandler(appError =>
{
    appError.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        var contextFeature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
        if (contextFeature != null)
        {
            Log.Error(contextFeature.Error, "Unhandled exception occurred");

            await context.Response.WriteAsync(new
            {
                message = "An unexpected error occurred. Please try again later.",
                statusCode = context.Response.StatusCode
            }.ToString() ?? string.Empty);
        }
    });
});

app.UseRouting();

app.MapControllers();

//Initialize database with simple data
using(var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
    await SeedDataAsync(context);
}

app.Run();

async Task SeedDataAsync(ProductDbContext context)
{
    if (!context.Products.Any())
    {
        var products = new[]
        {
         new Product
         {
             Id = 1,
             Name = "iPhone 16 Pro",
             Brand = "Apple",
             Price = 999.99m,
             CreateAt = DateTime.UtcNow,
             UpdateAt = DateTime.UtcNow
         },
         new Product
         {
             Id = 2,
             Name = "Galaxy S25",
             Brand = "Samsung",
             Price = 989.99m,
             CreateAt = DateTime.UtcNow,
             UpdateAt = DateTime.UtcNow
         },
         new Product
         {
             Id = 3,
             Name = "Pixel",
             Brand = "Google",
             Price = 899.99m,
             CreateAt = DateTime.UtcNow,
             UpdateAt = DateTime.UtcNow
         },
         new Product
         {
             Id = 4,
             Name = "Surface Pro 9",
             Brand = "Microsoft",
             Price = 1399.99m,
             CreateAt = DateTime.UtcNow,
             UpdateAt = DateTime.UtcNow
         },
         new Product
         {
             Id = 5,
             Name = "ThinkPad X1 Carbon",
             Brand = "Lenovo",
             Price = 1199.99m,
             CreateAt = DateTime.UtcNow,
             UpdateAt = DateTime.UtcNow
         }
     };
        context.Products.AddRange(products);
        await context.SaveChangesAsync();

        Log.Information("Database seeded with {Count} products", products.Length);
    }

    
}


