using System;

namespace TrafficTimetable.Infrastructure
{
    public class Response
    {
        public Response(string text, string[] buttons = null)
        {
            Text = text;
            Buttons = buttons;
        }

        public string Text { get; set; }
        public string[] Buttons { get; set; }
    }
}
