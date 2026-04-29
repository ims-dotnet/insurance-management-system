using InsureTrust.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
