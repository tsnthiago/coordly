using Microsoft.EntityFrameworkCore;
using ProductAPI.Middleware;
using ProductAPI.Repositories;
using Microsoft.OpenApi.Models;
using System.Reflection;
using ProductAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure DbContext for SQLite
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register IProductRepository
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// Configure Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ProductAPI", Version = "v1" });

    // Set the comments path for the Swagger JSON and UI.
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Adicione esta configuração antes de builder.Build()
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorOrigin",
        builder => builder
            .WithOrigins("http://localhost:5043", "http://127.0.0.1:5043")
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Comente ou remova esta linha
// app.UseHttpsRedirection();

// Certifique-se de que esta linha está presente e na ordem correta
app.UseCors("AllowBlazorOrigin");

app.UseAuthorization();

// Use the global exception handling middleware
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.MapControllers();

// Use CORS
app.UseCors("AllowBlazorOrigin");

// Ensure the database is created and migrations are applied
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();
}

app.Run();
