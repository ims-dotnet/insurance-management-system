using InsureTrust.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// ✅ Needed for ApiClient to access session/JWT later
builder.Services.AddHttpContextAccessor();

// ✅ Session support
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ✅ Authentication support for the frontend UI
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

// ✅ Typed HttpClients for specific services
builder.Services.AddHttpClient<IRenewalService, RenewalService>();
builder.Services.AddHttpClient<ISupportService, SupportService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// ✅ Session middleware must come before authentication/authorization
app.UseSession();

// ✅ Auth Mock — LOCAL DEV ONLY. Removed in staging/production.
// The Auth team's AccountController login will populate "JWToken" in other environments.
if (app.Environment.IsDevelopment())
{
    app.Use(async (context, next) =>
    {
        const string SessionKey = "JWToken";
        if (string.IsNullOrEmpty(context.Session.GetString(SessionKey)))
        {
            context.Session.SetString(SessionKey, "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjEiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiTWlua2kiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiJtYXlhbmtAZ21haWwuY29tIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiQ3VzdG9tZXIiLCJqdGkiOiI1ZjA1ZDk5YS04Y2ViLTQ2NjktOTA0NS1kYjFiM2VkNTJjNDIiLCJleHAiOjE3NzczNjk5MzEsImlzcyI6Ikluc3VyZVRydXN0LklkZW50aXR5U2VydmljZSIsImF1ZCI6Ikluc3VyZVRydXN0LkNsaWVudCJ9.ypkInz8FotpPTAZN4AzBvKsZWAwpkPmO0RTYqDm2glg");
        }
        await next();
    });
}

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();