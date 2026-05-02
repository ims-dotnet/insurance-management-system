using InsureTrust.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;


DotNetEnv.Env.TraversePath().Load();
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

builder.Services.AddHttpClient();

// ── HTTP Clients — all traffic flows through the YARP Gateway ─────────────────
var gatewayUrl = builder.Configuration["ApiBaseUrls:Gateway"]
    ?? throw new InvalidOperationException("ApiBaseUrls:Gateway is not configured.");

builder.Services.AddHttpClient<IAuthService, AuthService>(client =>
{
    client.BaseAddress = new Uri(gatewayUrl);
});

builder.Services.AddHttpClient<ICalculatorService, CalculatorService>(client =>
{
    client.BaseAddress = new Uri(gatewayUrl);
});

builder.Services.AddHttpClient<INotificationService, NotificationService>(client =>
{
    client.BaseAddress = new Uri(gatewayUrl);
});

builder.Services.AddHttpClient<IPolicyService, PolicyService>(client =>
{
    client.BaseAddress = new Uri(gatewayUrl);
});

builder.Services.AddHttpClient<IRenewalService, RenewalService>(client =>
{
    client.BaseAddress = new Uri(gatewayUrl);
});

builder.Services.AddHttpClient<ISupportService, SupportService>(client =>
{
    client.BaseAddress = new Uri(gatewayUrl);
});


builder.Services.AddHttpClient<IClaimService, ClaimService>(client =>
{
    client.BaseAddress = new Uri(gatewayUrl);
});

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.LoginPath = "/Account/Login";
            options.AccessDeniedPath = "/Account/AccessDenied";
        });

builder.Services.AddAuthorization();


var app = builder.Build();


// app.UseHttpsRedirection(); // Removed for Azure deployment consistency
app.UseStaticFiles();

app.UseSession();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();