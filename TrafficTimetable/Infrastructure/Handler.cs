using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using TrafficTimetable.Domain;

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
        private static Regex tagRegex = new Regex(@"хочу поехать|я еду");
        private static Regex positiveAnswerRegex = new Regex("да|ага|надо|согласен|хочу|продолжить", RegexOptions.IgnoreCase);
        private static Regex negativeAnswerRegex = new Regex("нет|не хочу|не нужно|не надо|откажусь|отмена", RegexOptions.IgnoreCase);
        private static Regex vowelRegex = new Regex("a|е|й");
        private static Regex directionRegex = new Regex("1|2|первое|второе");


        public static Tuple<string, string[]> Handle(string clientId, string sessionId, string command)
        {
            var clientState = Repository.GetClientState(clientId);

            if (clientState == null)
                clientState = Repository.CreateClientAndState(clientId, sessionId);

            if (clientState.SessionId != sessionId)
            {
                var stateInfo = Repository.GetMessageInWhichStateUser(clientId, sessionId);
                if (stateInfo != null) return Tuple.Create($"Кажется, что наша работа была прервана.Вы {stateInfo} Хотите продолжить?",
                    new string[2] { "Да", "Нет" });
            }

            if (clientState.WaitingToContinue)
            {
                var flag = IsPositiveOrNegativeAnswer(command);
                if(flag == "not_recgn") return Tuple.Create("Ой, кажется произошло недопонимание. " +
                    "Можете повторить свой запрос и выбрать из двух вариантов?", new string[2] { "Да", "Нет" });
                return Tuple.Create(Repository.ContinueWorkOrChangeToDefaultState(clientId, (flag == "1") ? true : false), new string[0]);
            }


            switch (clientState.ClientStatus)
            {
                case Status.AddingName:
                    return Tuple.Create(Repository.AddClientName(clientId,command), new string[0]);
                case Status.AddingStop:
                    return Tuple.Create(Repository.AddBufferStop(clientId, command), new string[0]);
                case Status.AddingTag:
                    var tag = tagRegex.Match(command).Value;
                    return Tuple.Create(Repository.AddBufferTag(clientId, tag), new string[0]);
                case Status.AddingRoute:
                    return Tuple.Create(Repository.FindRouteDirections(clientId, command),
                        new string[2] { "1", "2" });
                case Status.ChoosingDirection:
                    return (directionRegex.Match(command).Success)
                            ? Tuple.Create(Repository.AddStop(clientId, sessionId, command), new string[0])
                            : Tuple.Create("Пожалуйста, выберите между 1 или 2", new string[2] { "1", "2" });
            }

            if (helloRegex.Match(command).Success)
                return Tuple.Create($"Привет{GetClientName(clientId)}", new string[0]);
            if (stopRegex.Match(command).Success)
                return Tuple.Create(Repository.ChangeStateToAddStop(clientId, sessionId), new string[0]);
            return Tuple.Create("Увы,", new string[0]);
        }

        private static string GetClientName(string clientId)
        {
            var clientName = Repository.GetClientName(clientId);
            if (clientName == null)
                return Repository.ChangeStateToAddingName(clientId);
            return $", {clientName}!";
        }

        private static string IsPositiveOrNegativeAnswer(string command)
        {
            if (positiveAnswerRegex.Match(command).Success)
                return "1";
            else if (negativeAnswerRegex.Match(command).Success)
                return "0";
            return "not_recgn";
        }
    }
}
