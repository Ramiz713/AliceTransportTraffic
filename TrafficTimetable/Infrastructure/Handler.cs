using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace TrafficTimetable.Infrastructure
{
    public static class Handler
    {
        private static List<string> tags = new List<string> { "дом", "работа", "учёба" };
        private static Regex wordRegex = new Regex("[А-Я][а-яА-Я][^#&<>\"~;$^%{}?]{1,20}");
        private static Regex helloRegex = new Regex("привет|хай|как делишки|даров|здарова", RegexOptions.IgnoreCase);
        private static Regex stopRegex = new Regex("хочу добавить остановку", RegexOptions.IgnoreCase);
        private static Regex numberRegex = new Regex(@"[\d]|[\d][\d]|[\d][\d][\D]");
        private static Regex tagRegex = new Regex(@"дом|работ[а-я]|учёб[а-я]");

        public static string Handle(string clientId, string sessionId, string command)
        {
            if (command == "1" || command == "2")
                return Repository.AddStop(clientId, command);
            if (helloRegex.Match(command).Success)
                return $"Привет{FindClient(clientId)}";
            if (tagRegex.Match(command).Success)
            {
                var tag = tagRegex.Match(command).Value;
                return Repository.AddBufferTag(clientId, tag);
            }
            if (stopRegex.Match(command).Success)
                return "Назовите название остановки";
            if (wordRegex.Match(command).Success)
            {
                if (Repository.IsClientExist(clientId))
                    return Repository.AddBufferStop(clientId, command);
                else
                    return Repository.AddClient(clientId, command);
            }
            if (numberRegex.Match(command).Success)
                return req.Reply(Repository.FindRouteDirections(clientId, command),
                    buttons: new ButtonModel[]
                    {
                    new ButtonModel() { Title = "1", Hide = true },
                    new ButtonModel() { Title = "2", Hide = true }
                    });
            return req.Reply("Ничего");
        }

        private static string FindClient(string clientId)
        {
            var clientName = Repository.GetClientName(clientId);
            return (clientName == null)
                ? "! Кажется, я вас вижу, ой, слышу впервые... давайте знакомиться! Как вас зовут?"
                : $", {clientName}!";
        }
    }
}
