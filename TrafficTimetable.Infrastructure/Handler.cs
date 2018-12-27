using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TrafficTimetable.Domain;

namespace TrafficTimetable.Infrastructure
{
    public static class Handler
    {
        public static Response Handle(string clientId, string sessionId, string command)
        {
            var clientState = Repository.GetClientState(clientId);

            if (clientState == null)
                return Repository.CreateClientAndState(clientId, sessionId);

            if (clientState.WaitingToContinue)
            {
                var flag = IsPositiveOrNegativeAnswer(command);
                if (flag == "U") return new Response("Ой, кажется произошло недопонимание. " +
                     "Можете повторить свой запрос и выбрать из двух вариантов?", new string[2] { "Да", "Нет" });
                return Repository.ContinueWorkOrChangeToDefaultState(clientId, (flag == "Y") ? true : false);
            }

            if (clientState.SessionId != sessionId)
                return Repository.GetResponseUserState(clientId, sessionId);

            if (Regexes.negativeAnswerRegex.Match(command).Success && clientState.ClientStatus != Status.Default
                && clientState.ClientStatus != Status.AddingName)
                return Repository.ReturnDafaultState(clientId, "Сказано - не сделано!");

            switch (clientState.ClientStatus)
            {
                case Status.AddingName:
                    if (Regexes.negativeAnswerRegex.Match(command).Success)
                        return Repository.AutoGenerateClientName(clientId);
                    return Repository.AddClientName(clientId, command);
                case Status.AddingStop:
                    return Repository.AddBufferStop(clientId, command);
                case Status.AddingTag:
                    return Repository.AddBufferTag(clientId, command);
                case Status.AddingRoute:
                    return Repository.FindRouteDirections(clientId, command);
                case Status.ChoosingDirection:
                    return Regexes.directionRegex.Match(command).Success
                            ? Repository.AddStop(clientId, command)
                            : new Response("Пожалуйста, выберите между 1 или 2", new string[2] { "1", "2" });
            }

            if (Regexes.showAllStopsRegex.Match(command).Success)
                return Repository.ShowSavedStops(clientId);

            if (Regexes.tagRegex.Match(command).Success)
                return Repository.GetTimeByTag(clientId, GetTag(command));
            if (Regexes.routeAddingRegex.Match(command).Success)
            {
                var match = Regexes.routeRegex.Match(command);
                if (match.Success)
                    return Repository.AddRouteToTag(clientId, GetTag(command), match.Value.Remove(match.Value.Length - 1));
            }
            if (Regexes.stopAddingRegex.Match(command).Success)
            {
                var words = command.Split(' ');
                var stop = words[words.Length - 1];
                return (stop != "остановку")
                    ? Repository.AddBufferStop(clientId, stop)
                    : Repository.ChangeStateToAddStop(clientId);
            }
            if (Regexes.tatarRegex.Match(command).Success)
                return new Response("Абау, сездә татарча беләсез мәллә? Әфәрин!");

            if (Regexes.helloRegex.Match(command).Success)
                return new Response($"Привет{GetClientName(clientId)}");

            return new Response("Произошло недопонимание");
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
            if (Regexes.positiveAnswerRegex.Match(command).Success)
                return "Y";
            else if (Regexes.negativeAnswerRegex.Match(command).Success)
                return "N";
            return "U";
        }

        private static string GetTag(string command)
        {
            var words = command.Split(' ');
            var tag = words[words.Length - 1];
            return tag.Remove(tag.Length - 1);
        }
    }
}
