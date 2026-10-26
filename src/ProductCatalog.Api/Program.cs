using ProductCatalog.Api.Filters;

var builder = WebApplication.CreateBuilder(args);

// Locally the demo key comes from appsettings.Development.json, in CI from the ApiKey environment variable.
// User secrets and environment variables override the file, so you can use your own key.
if (string.IsNullOrWhiteSpace(builder.Configuration["ApiKey"]))
{
    throw new InvalidOperationException(
        "ApiKey is not set. Run in the Development environment, or set ApiKey in user secrets or an environment variable.");
}

builder.Services.AddControllers(options => options.Filters.Add<ApiKeyFilter>());

var app = builder.Build();

app.MapControllers();

await app.RunAsync();
