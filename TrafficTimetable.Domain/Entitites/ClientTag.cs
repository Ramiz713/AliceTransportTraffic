using System;
using System.Collections.Generic;
using System.Text;

namespace TrafficTimetable.Domain
{
    public class ClientTag
    {
        public ClientTag(string clientId, string tagName, string stopId)
        {
            ClientId = clientId;
            TagName = tagName;
            StopId = stopId;
            Routes = new List<string>();
        }

        public string ClientId { get; private set; }
        public Client Client { get; set; }

        public string StopId { get; private set; }
        public Stop Stop { get; set; }

        public List<string> Routes { get; private set; }

        public string TagName { get; set; }
    }
}
