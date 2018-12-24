using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        }
        public string ClientId { get; private set; }
        public Client Client { get; set; }

        public string StopId { get; private set; }
        public Stop Stop { get; set; }

        public string TagName { get; set; }

    }
}
