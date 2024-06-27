using TestImageApi.DB;

namespace TestImageApi.Services;

public class IPDBService(ImageDB _db)
{
    public int GetIdByIPAddress(string ip)
    {
        Ip? ipObj = _db.Ips.FirstOrDefault(x => x.Address == ip);
        if (ipObj == null)
        {
            ipObj = new Ip { Address = ip, CreateTime = DateTime.Now };
            _db.Ips.Add(ipObj);
            _db.SaveChanges();
        }
        return ipObj.Id;
    }
}
