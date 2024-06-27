using Microsoft.Extensions.Primitives;

namespace TestImageApi.Services;

public class ClientIPService(IHttpContextAccessor ctx)
{
    public string GetIPAddress()
    {
        if (ctx.HttpContext!.Request.Headers.TryGetValue("X-Forwarded-For", out StringValues ip))
        {
            return ip.ToString();
        }
        else
        {
            return ctx.HttpContext.Connection.RemoteIpAddress!.ToString();
        }
    }
}
