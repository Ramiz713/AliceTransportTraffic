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
        [Key]
        public string ClientId { get; set; }

        [Key]
        public string TagName { get; set; }
        public string StopId { get; set; }
    }
}
