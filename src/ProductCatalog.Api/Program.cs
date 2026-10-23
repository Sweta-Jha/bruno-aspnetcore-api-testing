using ProductCatalog.Api.Filters;

var builder = WebApplication.CreateBuilder(args);

// Locally the key comes from user secrets, in CI from the ApiKey environment variable.
if (string.IsNullOrWhiteSpace(builder.Configuration["ApiKey"]))
{
    throw new InvalidOperationException(
        "ApiKey is not set. Run: dotnet user-secrets set ApiKey local-dev-key --project src/ProductCatalog.Api " +
        "(use the same key as API_KEY in api-tests/.env)");
}

builder.Services.AddControllers(options => options.Filters.Add<ApiKeyFilter>());

var app = builder.Build();

app.MapControllers();

await app.RunAsync();
