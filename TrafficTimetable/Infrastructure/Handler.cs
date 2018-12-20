using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace TrafficTimetable.Infrastructure
{
    public static class Handler
    {
        private static string greeting = "Привет! Этот навык может быть полезен для быстрого получения информации о времени прибытия транспорта к остановке";
        private static string firstMeeting = "! Кажется, я вас вижу, ой, слышу впервые... давайте знакомиться! Как вас зовут?";

        private static List<string> tags = new List<string> { "дом", "работа", "учёба" };
        private static Regex wordRegex = new Regex("[А-Я][а-яА-Я][^#&<>\"~;$^%{}?]{1,20}");
        private static Regex helloRegex = new Regex("привет|хай|как делишки|даров|здарова", RegexOptions.IgnoreCase);
        private static Regex stopRegex = new Regex("хочу добавить остановку", RegexOptions.IgnoreCase);
        private static Regex numberRegex = new Regex(@"[\d]|[\d][\d]|[\d][\d][\D]");
        private static Regex tagRegex = new Regex(@"хочу поехать|еду на|еду||");


        public static Tuple<string, string[]> Handle(string clientId, string sessionId, string command)
        {
            var clientState = Repository.GetClientState(clientId, sessionId);

            if (helloRegex.Match(command).Success)
                return Tuple.Create($"Привет{FindClient(clientId, sessionId)}", new string[0]);
            if (clientState == null)
                clientState = Repository.CreateClientState(clientId, sessionId);
            if (clientState.IsAddName)
                return Tuple.Create(Repository.AddClientName(clientId, command), new string[0]);

            if (clientState.IsAddStop)
                return Tuple.Create(Repository.AddBufferStop(clientId, command), new string[0]);

            if (clientState.IsAddTag)
            {
                var tag = tagRegex.Match(command).Value;
                return Tuple.Create(Repository.AddBufferTag(clientId, tag), new string[0]);
            }

            if (clientState.IsAddRoute)
                return Tuple.Create(Repository.FindRouteDirections(clientId, command),
                    new string[2] { "1", "2" });

            if (clientState.IsChoosingDirection)
                return (command != "1" && command != "2")
                    ? Tuple.Create("Пожалуйста, выберите между 1 или 2", new string[2] { "1", "2" })
                    : Tuple.Create(Repository.AddStop(clientId, sessionId, command), new string[0]);

            if (stopRegex.Match(command).Success)
                return Tuple.Create(Repository.ChangeStateToAddStop(clientId), new string[0]);

            return Tuple.Create("Ничего", new string[0]);
        }

        private static string FindClient(string clientId, string sessionId)
        {
            var clientName = Repository.GetClientName(clientId);
            if (clientName == null)
                return Repository.ChangeStateToAddNameAndAddClient(clientId, sessionId);
            return $", {clientName}!";
        }
    }
}
