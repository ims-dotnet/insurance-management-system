DotNetEnv.Env.TraversePath().Load();
var builder = WebApplication.CreateBuilder(args);

// ── YARP Reverse Proxy ────────────────────────────────────────────────────────
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .ConfigureHttpClient((context, handler) =>
    {
        // Only bypass SSL in Development (local dev with self-signed certs)
        if (builder.Environment.IsDevelopment())
        {
            var h = (object)handler;
            if (h is SocketsHttpHandler socketsHandler)
            {
                socketsHandler.SslOptions.RemoteCertificateValidationCallback = (message, cert, chain, errors) => true;
            }

            if (h is HttpClientHandler clientHandler)
            {
                clientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            }
        }
    });

// ── CORS ─────────────────────────────────────────────────────────────────────
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options =>
{
    options.AddPolicy("GatewayCors", policy =>
    {
        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddAuthorization();

// ── Swagger (G4) ─────────────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "InsureTrust Gateway", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();
app.UseCors("GatewayCors");
app.UseAuthorization();

// ── Map YARP ─────────────────────────────────────────────────────────────────
app.MapReverseProxy();

app.Run();
