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
                if (flag == "not_recgn") return new Response("Ой, кажется произошло недопонимание. " +
                     "Можете повторить свой запрос и выбрать из двух вариантов?", new string[2] { "Да", "Нет" });
                return Repository.ContinueWorkOrChangeToDefaultState(clientId, (flag == "1") ? true : false);
            }

            if (clientState.SessionId != sessionId)
            {
                var stateInfo = Repository.GetResponseUserState(clientId, sessionId);
                if (stateInfo != null) return stateInfo;
            }

            if (Regexes.negativeAnswerRegex.Match(command).Success && clientState.ClientStatus != Status.Default
                && clientState.ClientStatus != Status.AddingName)
                return Repository.ReturnDafaultState(clientId);

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
                    return (Regexes.directionRegex.Match(command).Success)
                            ? Repository.AddStop(clientId, command)
                            : new Response("Пожалуйста, выберите между 1 или 2", new string[2] { "1", "2" });
            }

            if (Regexes.showAllStopsRegex.Match(command).Success)
                return Repository.ShowSavedStops(clientId);

            if (Regexes.tagRegex.Match(command).Success)
            {
                var words = command.Split(' ');
                var tag = words[words.Length - 1].Substring(0, 3);
                return new Response(Repository.GetTimeByTag(clientId, tag));
            }
            if (Regexes.stopAddingRegex.Match(command).Success)
                return Repository.ChangeStateToAddStop(clientId);
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
                return "1";
            else if (Regexes.negativeAnswerRegex.Match(command).Success)
                return "0";
            return "not_recgn";
        }
    }
}
