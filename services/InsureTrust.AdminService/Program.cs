using InsureTrust.AdminService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ================= SERILOG CONFIGURATION =================
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/admin-service-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// ================= CONTROLLERS & VALIDATION =================
builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// ================= SWAGGER =================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ================= HTTP CLIENT ( IMPORTANT) =================
builder.Services.AddHttpClient<IAdminService, AdminService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5000/"); 
});

// ================= JWT AUTH =================
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("THIS_IS_SECRET_KEY_CHANGE_IT") // same as Identity Service
            )
        };
    });

var app = builder.Build();

// ================= MIDDLEWARE PIPELINE =================
app.UseMiddleware<InsureTrust.AdminService.Middleware.ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseSerilogRequestLogging();

// 🔥 IMPORTANT ORDER
app.UseAuthentication();   // MUST BEFORE Authorization
app.UseAuthorization();

app.MapControllers();

app.Run();