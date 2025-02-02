using GroceryOptimizerApi.Data;
using GroceryOptimizerApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

// Determine which .env file to load based on ASPNETCORE_ENVIRONMENT
var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
var envFile = env == "Production" ? ".env.production" : ".env.development";
envFile = ".env.production"; // OVERRIDE production database connectionstring data
Console.WriteLine("************* Config from: " + envFile);

// Load environment variables from the correct .env file
Env.Load(envFile);

// Construct connectionstrig from env
var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
var dbPort = Environment.GetEnvironmentVariable("DB_PORT");
var dbName = Environment.GetEnvironmentVariable("DB_NAME");
var dbUser = Environment.GetEnvironmentVariable("DB_USER");
var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");
var connectionString = $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPassword}";

// Add services to the container.
// Add services to the container. builder.Services.AddControllers();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    //options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
    options.UseNpgsql(connectionString)
);

builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

builder.Services.AddControllers();

// Cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendReactApp",
        builder => builder
        .WithOrigins("http://localhost:3000")
        .AllowAnyHeader()
        .AllowAnyOrigin());
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication().AddBearerToken();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("FrontendReactApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
