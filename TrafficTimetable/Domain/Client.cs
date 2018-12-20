using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrafficTimetable.Domain
{
    public class Client
    {
        private Client() { }
        public Client(string id) => Id = id;

        public string Id { get; private set; }
        public string Name { get; set; }
    }
}
