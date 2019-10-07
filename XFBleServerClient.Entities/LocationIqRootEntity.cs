using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace XFBleServerClient.Entities
{
    public class LocationIqRootEntity
    {
        [JsonProperty("place_id")]
        public string PlaceId { get; set; }

        [JsonProperty("osm_type")]
        public string OsmType { get; set; }

        [JsonProperty("osm_id")]
        public string OsmId { get; set; }

        [JsonProperty("licence")]
        public Uri Licence { get; set; }

        [JsonProperty("lat")]
        public string Lat { get; set; }

        [JsonProperty("lon")]
        public string Lon { get; set; }

        [JsonProperty("display_name")]
        public string DisplayName { get; set; }

        [JsonProperty("boundingbox")]
        public List<string> Boundingbox { get; set; }

        [JsonProperty("importance")]
        public double Importance { get; set; }

        [JsonProperty("address")]
        public LocationIqAddressEntity Address { get; set; }
    }
  
}
