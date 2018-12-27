using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace TrafficTimetable.Infrastructure
{
    public static class Regexes
    {
        public static Regex helloRegex = new Regex("привет|хай|как делишки|даров|здарова", RegexOptions.IgnoreCase);
        public static Regex stopAddingRegex = new Regex("хочу добавить остановку|добавить остановку", RegexOptions.IgnoreCase);
        public static Regex routeAddingRegex = new Regex("хочу добавить маршрут|добавь маршрут", RegexOptions.IgnoreCase);
        public static Regex routeRegex = new Regex(@"[\d] |[\d][\d] |[\d][\d][\D] ", RegexOptions.IgnoreCase);
        public static Regex tagRegex = new Regex(@"хочу поехать|я еду|я направляюсь|поехали|направляемся|еду", RegexOptions.IgnoreCase);
        public static Regex positiveAnswerRegex = new Regex("да|ага|надо|согласен|хочу|продолжить", RegexOptions.IgnoreCase);
        public static Regex negativeAnswerRegex = new Regex("нет|не хочу|не нужно|не надо|откажусь|отмена", RegexOptions.IgnoreCase);
        public static Regex showAllStopsRegex = new Regex("покажи все мои остановки|покажи мои остановки|покажи мои теги|мои остановки|мои теги", RegexOptions.IgnoreCase);
        public static Regex tatarRegex = new Regex("салям|исэнмесез|salam|сэлам", RegexOptions.IgnoreCase);
        public static Regex directionRegex = new Regex("1|2|первое|второе", RegexOptions.IgnoreCase);
        public static Regex nounRegex = new Regex("a|я", RegexOptions.IgnoreCase);
    }
}
