using Microsoft.OpenApi.Models;
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

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TPI Programación 4 - E-commerce API",
        Version = "v1",
        Description = "API para el TP de Programación 4"
    });

    // Configuración para JWT en Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingresá el token JWT con el prefijo Bearer. Ejemplo: Bearer {token}"
    });

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


 builder.Services.AddAuthorization();



var app = builder.Build();

// Ejecutar seeder cuando inicia la aplicación(Cambiar antes del deploy)
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
    var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
    DbSeeder.Seed(dbContext, configuration);
}

// Intercepta cualquier error globalmente antes de procesar las peticiones
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TPI Programación 4 - E-commerce API V1");
        c.RoutePrefix = "swagger";
    });
}



app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();