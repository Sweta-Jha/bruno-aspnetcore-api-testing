using Microsoft.OpenApi;
using ProductCatalog.Api.Filters;

var builder = WebApplication.CreateBuilder(args);

// Locally the key comes from user secrets, in CI from the ApiKey environment variable.
if (string.IsNullOrWhiteSpace(builder.Configuration["ApiKey"]))
{
    throw new InvalidOperationException(
        "ApiKey is not set. Run: dotnet user-secrets set ApiKey <your-key> --project src/ProductCatalog.Api");
}

builder.Services.AddControllers(options => options.Filters.Add<ApiKeyFilter>());
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Adds an Authorize button to Swagger UI so "Try it out" can send the key.
    options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Name = ApiKeyFilter.HeaderName
    });
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("ApiKey", document)] = []
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

await app.RunAsync();
