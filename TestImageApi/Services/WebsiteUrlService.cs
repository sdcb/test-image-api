using System.Web;

namespace TestImageApi.Services;

public class WebsiteUrlService(HostUrlService _my)
{
    public string GetImageUrl(int id, string fileName)
    {
        return $"{_my.GetWebsiteUrl()}/api/image/{id}/{HttpUtility.UrlEncode(fileName)}";
    }

    public string GetZipUrl(int id, string fileName)
    {
        return $"{_my.GetWebsiteUrl()}/api/zip/{id}/{HttpUtility.UrlEncode(fileName)}";
    }
}