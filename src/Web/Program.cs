using System.Text;
using Application.Security;
using Application.Services;
using Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Web.Middlewares; // Namespace del middleware global de excepciones

var builder = WebApplication.CreateBuilder(args);

// Add services to the container

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Cadena de conexion a SQLite
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                        ?? "Data Source=products.db";

// Registro del DbContext (Infrastructure)
builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseSqlite(connectionString));

// Registro del Repositorio (Inyeccion de dependencias)
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Repositorio genérico 
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Registro de Servicios (Application)
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<ProductService>();

// Registro del middleware para el manejo de excepciones
builder.Services.AddTransient<GlobalExceptionHandlingMiddleware>();

// Configuracion JWT
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

// Servicios de autenticacion 
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<AuthService>();

// Configuracion de autenticacion con JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()!;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
            ClockSkew = TimeSpan.Zero
        };

    });

 builder.Services.AddAuthorization();



var app = builder.Build();

// Intercepta cualquier error globalmente antes de procesar las peticiones
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "My API V1");
    });
}



app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();