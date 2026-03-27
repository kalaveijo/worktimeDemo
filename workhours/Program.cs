var builder = WebApplication.CreateBuilder(args);

var configuredOrigins = builder.Configuration.GetSection("CORS:AllowedOrigins").Get<string[]>() ?? [];
var envOrigins = (builder.Configuration["CORS_ALLOWED_ORIGINS"] ?? string.Empty)
    .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
var allowedOrigins = configuredOrigins
    .Concat(envOrigins)
    .Distinct(StringComparer.OrdinalIgnoreCase)
    .ToArray();

builder.Services.AddCors(options =>
{
    options.AddPolicy("LocalFrontend", policy =>
    {
        if (allowedOrigins.Length == 0)
        {
            return;
        }

        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

if (allowedOrigins.Length > 0)
{
    app.UseCors("LocalFrontend");
}

app.MapGet("/", () => "Hello World!");

app.Run();
