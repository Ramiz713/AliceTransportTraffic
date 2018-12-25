using System;

namespace TrafficTimetable.Infrastructure
{
    class Response
    {
        public Response(string text, string[] buttons)
        {
            Text = text;
            Buttons = buttons;
        }
        private string Text { get; set; }
        private string[] Buttons { get; set; }
    }
}
