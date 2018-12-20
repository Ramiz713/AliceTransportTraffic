using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TrafficTimetable.Domain
{
    public class ClientTag
    {
        [Key]
        public string ClientId { get; set; }

        [Key]
        public string TagName { get; set; }
        public string StopId { get; set; }
    }
}
