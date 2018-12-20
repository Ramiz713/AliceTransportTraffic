using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TrafficTimetable.Domain
{
    public class State
    {
        public State(int clientId)
        {
            ClientId = ClientId;
            IsDefault = true;
        }

        [Key]
        public string ClientId { get; set; }

        public bool IsDefault { get; set; }
        public bool IsAddStop { get; set; }
        public bool IsAddRoute { get; set; }
        public bool IsAddTag { get; set; }
        public string BufferStopName { get; set; }
        public string BufferRouteName { get; set; }
        public string BufferTagName { get; set; }

    }
}
