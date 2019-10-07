using System.Threading.Tasks;

namespace XFBleServerClient.Managers
{
    public interface ILocationManager
    {
        Task<string> GetLocationAddress(double latitude, double longitude);
    }
}