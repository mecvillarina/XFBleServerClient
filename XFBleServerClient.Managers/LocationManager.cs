using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using XFBleServerClient.Entities;

namespace XFBleServerClient.Managers
{
    public class LocationManager : ILocationManager
    {
        private const string _baseUriString = "https://us1.locationiq.com/";
        private const string _privateKey = "pk.889730e06d5b977081f78fe552dd25d1";

        public async Task<string> GetLocationAddress(double latitude, double longitude)
        {
            //Refactor
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_baseUriString);

                string requestUri = $"/v1/reverse.php?key={_privateKey}&lat={latitude}&lon={longitude}&format=json";

                var responseMessage = await client.GetAsync(requestUri);

                responseMessage.EnsureSuccessStatusCode();

                string responseContent = await responseMessage.Content.ReadAsStringAsync();
                var rootEntity = JsonConvert.DeserializeObject<LocationIqRootEntity>(responseContent);

                return rootEntity.DisplayName;
            }
        }
    }
}
