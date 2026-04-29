using InsureTrust.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;


DotNetEnv.Env.TraversePath().Load();
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();
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
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = 401;
            return Task.CompletedTask;
        };

        options.Events.OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = 403;
            return Task.CompletedTask;
        };
    });

builder.Services.AddAuthorization();


var app = builder.Build();


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();


app.UseSession();
app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();