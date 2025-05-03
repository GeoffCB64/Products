using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Products.Data;
using Products.Services;
using Products.Models;
using System.ComponentModel.DataAnnotations;
using Products.Services.Auth;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddDbContext<ProductsDbContext>(opt =>
    opt.UseInMemoryDatabase("ProductsDb"));

builder.Services.AddScoped<IProductService, ProductService>();

#warning TODO: Replace DevTestUserOnlyValidator with a real implementation before deploying to production.
builder.Services.AddSingleton<IUserValidator, DevTestUserOnlyValidator>();

// JWT Authentication
#warning TODO: Retrieve jwtKey in a managed secrets service or something similar
var jwtKey = "testonly_secret_key_1234567890123456"; 
var keyBytes = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes)
        };
    });

builder.Services.AddAuthorization();

// Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Products API", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();


//----------------------
// Endpoints
//----------------------

// Anonymous health check
/* If there was a database involved this could be extended to add in
   app.MapGet("/health", async (ProductsDbContext db) =>
   {
       var canConnect = await db.Database.CanConnectAsync();
   
       return canConnect
           ? Results.Ok("Healthy")
           : Results.Problem("Database unreachable", statusCode: 503);
   });
*/
app.MapGet("/health", () => Results.Ok("Healthy"));

// Public login endpoint (returns JWT for any username)
app.MapPost("/login", (LoginRequest request, IUserValidator validator) =>
{
    if (!validator.ValidateCredentials(request.Username, request.Password))
    {
        return Results.Unauthorized();
    }

    var claims = new[] { new Claim(ClaimTypes.Name, request.Username) };

    var token = new JwtSecurityToken(
        claims: claims,
        expires: DateTime.UtcNow.AddHours(1),
        signingCredentials: new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            SecurityAlgorithms.HmacSha256
        )
    );

    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
    return Results.Ok(new { token = tokenString });
});


// Protected endpoint to create a product (assumes valid JSON and types)
app.MapPost("/products", async (CreateProductDto dto, IProductService svc) =>
{
    var validationContext = new ValidationContext(dto);
    var validationResults = new List<ValidationResult>();

    if (!Validator.TryValidateObject(dto, validationContext, validationResults, true))
    {
        var errorDict = validationResults
            .GroupBy(r => r.MemberNames.FirstOrDefault() ?? "")
            .ToDictionary(
                g => g.Key,
                g => g.Select(r => r.ErrorMessage ?? "Invalid value.").ToArray()
            );

        return Results.ValidationProblem(errorDict);
    }

    var product = new Product(
        Id: 0,
        Name: dto.Name,
        Colour: dto.Colour,
        Price: dto.Price,
        CreatedDate: DateTime.UtcNow
    );

    var created = await svc.CreateAsync(product);
    return Results.Created($"/products/{created.Id}", created);
}).RequireAuthorization();


// Protected endpoint to retrieve products (with optional colour filter)
app.MapGet("/products", async (string? colour, IProductService svc) =>
{
    return string.IsNullOrEmpty(colour)
        ? Results.Ok(await svc.GetAllAsync())
        : Results.Ok(await svc.GetByColourAsync(colour));
}).RequireAuthorization();

app.Run();

// Required for xUnit
public partial class Program { }
