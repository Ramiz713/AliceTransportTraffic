using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrafficTimetable
{
    public class Client
    {
        private Client() { }
        public Client(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public string Id { get; private set; }
        public string Name { get; set; }

        [NotMapped]
        public Dictionary<string, string> Directions;
        [NotMapped]
        public string BufferStopName;
        [NotMapped]
        public string BufferRouteName;
        [NotMapped]
        public string BufferTagName;
    }
}
