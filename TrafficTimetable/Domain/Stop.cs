﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TrafficTimetable.Domain
{
    public class Stop
    {
        public Stop(string id, string name, string url)
        {
            Id = id;
            Name = name;
            Url = url;
            Routes = new List<string>();
        }

        public string Id { get; private set; }
        public string Name { get; private set; }
        public string Url { get; private set; }
        public List<string> Routes { get; private set; }
    }
}
