using System;
using System.Collections.Generic;
using System.Text;

namespace TrafficTimetable.Domain
{
    public enum Status
    {
        Default,
        AddingName,
        AddingStop,
        AddingTag,
        AddingRoute,
        AddingOtherRoute,
        ChoosingDirection
    }
}
