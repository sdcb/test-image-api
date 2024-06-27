using Microsoft.Extensions.Primitives;

namespace TestImageApi.Services;

public class HostUrlService(IHttpContextAccessor _ctx)
{
    public string GetWebsiteUrl()
    {
        HttpRequest request = _ctx.HttpContext!.Request;
        IHeaderDictionary headers = request.Headers;

        string scheme = headers.TryGetValue("X-Forwarded-Proto", out StringValues schemeValue) ? schemeValue.FirstOrDefault()! : request.Scheme;
        string host = headers.TryGetValue("X-Forwarded-Host", out StringValues hostValue) ? hostValue.FirstOrDefault()! : request.Host.ToString();

        string url = $"{scheme}://{host}";

        return url;
    }
}
