using Microsoft.EntityFrameworkCore;
using InsureTrust.ClaimService.Data;
using InsureTrust.ClaimService.Repositories;
using InsureTrust.ClaimService.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Serilog;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

DotNetEnv.Env.TraversePath().Load();
var builder = WebApplication.CreateBuilder(args);

// ================= SERILOG CONFIGURATION =================
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddAutoMapper(typeof(Program));

// ================= JWT AUTH =================
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key missing.");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer missing.");
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? throw new InvalidOperationException("Jwt:Audience missing.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

// DB
builder.Services.AddDbContext<ClaimDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// DI
builder.Services.AddScoped<IClaimRepository, ClaimRepository>();
builder.Services.AddScoped<IClaimService, ClaimService>();

var app = builder.Build();

// ================= MIDDLEWARE PIPELINE =================
app.UseMiddleware<InsureTrust.ClaimService.Middleware.ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseSerilogRequestLogging();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();