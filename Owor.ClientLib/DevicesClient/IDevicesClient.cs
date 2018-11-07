using System.Net.Http;
using System.Threading.Tasks;
using Owor.Shared;

namespace Owor.ClientLib.Devices
{

    public interface IDevicesClient
    {

        Task<OwDeviceDto[]> GetDevicesAsync();

        Task<OwDeviceDto> GetDeviceAsync(string deviceId);

    }

}