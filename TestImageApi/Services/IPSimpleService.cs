namespace TestImageApi.Services;

public class IPSimpleService(ClientIPService _ip, IPDBService _db)
{
    public int GetIPId()
    {
        string ip = _ip.GetIPAddress();
        return _db.GetIdByIPAddress(ip);
    }
}
