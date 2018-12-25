using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TrafficTimetable.Domain
{
    public class Bus
    {
        [JsonProperty("GaragNumb")]
        public int Id { get; set; }

        [JsonProperty("Marsh")]
        public string Route { get; set; }

        [JsonProperty("Graph")]
        public long Graph { get; set; }

        [JsonProperty("Smena")]
        public long Shift { get; set; }

        [JsonProperty("TimeNav")]
        public string TimeNavigation { get; set; }

        [JsonProperty("Latitude")]
        public string Latitude { get; set; }

        [JsonProperty("Longitude")]
        public string Longitude { get; set; }

        [JsonProperty("Speed")]
        public long Speed { get; set; }

        [JsonProperty("Azimuth")]
        public long Azimuth { get; set; }
    }

    public class BusTime
    {
        [JsonProperty("updated_at")]
        public string Time { get; set; }

        [JsonProperty("data")]
        public Bus Bus { get; set; }
    }
}
