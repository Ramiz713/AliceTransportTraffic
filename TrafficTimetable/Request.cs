using System;
using System.Collections.Generic;
using System.Text;

namespace TrafficTimetable.Infrastructure
{
    class Request
    {
        public string SessionId { get; set; }
        public string ClientId { get; set; }
        public string Command { get; set; }
    }
}
