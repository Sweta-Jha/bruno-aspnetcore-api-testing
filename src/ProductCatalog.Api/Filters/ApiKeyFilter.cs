using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ProductCatalog.Api.Filters;

/// <summary>Returns 401 unless the request sends the right key in the <c>X-Api-Key</c> header.</summary>
/// <remarks>A single shared key keeps the demo short. Real APIs usually use OAuth or JWT bearer tokens.</remarks>
public class ApiKeyFilter(IConfiguration configuration) : IAuthorizationFilter
{
    public const string HeaderName = "X-Api-Key";

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var expected = Encoding.UTF8.GetBytes(configuration["ApiKey"]!);
        var provided = Encoding.UTF8.GetBytes(context.HttpContext.Request.Headers[HeaderName].ToString());

        // FixedTimeEquals takes the same time whether the first or the last character is wrong,
        // so the response time does not give the key away.
        if (!CryptographicOperations.FixedTimeEquals(expected, provided))
        {
            context.Result = new UnauthorizedResult();
        }
    }
}
