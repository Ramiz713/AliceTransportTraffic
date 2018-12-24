using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using TrafficTimetable.Domain;

namespace TrafficTimetable.Infrastructure
{
    internal static class Repository
    {
        public static void Main() { }

        private static List<string> nicknames = new List<string> { "Кабанчик", "Хитрый татарин", "Рыночный клуб",
            "Первичный череп", "Философский женя","Сложный пришелец" };

        public static string ShowSavedStops(string clientId)
        {
            using (ClientDataContext db = new ClientDataContext())
            {
                var clientStops = db.ClientTags.Where(c => c.ClientId == clientId)
                    .Join(db.Stops, tag => tag.StopId, stop => stop.Id,
                    (tag, stop) => new { tag.TagName, stop.Name, stop.Routes });
                StringBuilder result = new StringBuilder().Append("Все ваши сохраненные остановки:\n");
                foreach (var stop in clientStops)
                    result
                        .Append("Остановка: ")
                        .Append(stop.Name)
                        .Append("\n")
                        .Append("Тег остановки: ")
                        .Append(stop.TagName)
                        .Append("\n")
                        .Append("Маршруты: ")
                        .Append(string.Join(", ", stop.Routes))
                        .Append("\n\n");
                return result.ToString();
            }
        }

        public static ClientState CreateClientAndState(string clientId, string sessionId)
        {
            using (ClientDataContext db = new ClientDataContext())
            {
                var clientState = new ClientState(clientId, sessionId);
                db.ClientStates.Add(clientState);
                db.Clients.Add(new Client(clientId));
                db.SaveChanges();
                return clientState;
            }
        }

        public static ClientState GetClientState(string clientId)
        {
            using (ClientDataContext db = new ClientDataContext())
                return db.ClientStates
                    .FirstOrDefault(c => c.ClientId == clientId);
        }

        public static string GetMessageInWhichStateUser(string clientId, string newSessionId)
        {
            using (ClientDataContext db = new ClientDataContext())
            {
                var clientState = GetClientState(clientId);
                clientState.SessionId = newSessionId;
                if (clientState.ClientStatus != Status.Default) clientState.WaitingToContinue = true;
                db.ClientStates.Update(clientState);
                db.SaveChanges();
                return clientState.GetStateInfo();
            }
        }

        public static string ContinueWorkOrChangeToDefaultState(string clientId, bool flag)
        {
            using (ClientDataContext db = new ClientDataContext())
            {
                var clientState = GetClientState(clientId);
                var response = clientState.GetInstruction();
                if (!flag)
                {
                    clientState.ClientStatus = Status.Default;

                    response = @"Хорошо, как говорится, забудем ""старое"")";
                }
                clientState.WaitingToContinue = false;
                db.ClientStates.Update(clientState);
                db.SaveChanges();
                return response;
            }
        }

        public static string ChangeStateToAddingName(string clientId)
        {
            using (ClientDataContext db = new ClientDataContext())
            {
                var clientState = GetClientState(clientId);
                clientState.ClientStatus = Status.AddingName;
                db.ClientStates.Update(clientState);
                db.SaveChanges();
                return clientState.GetInstruction();
            }
        }

        public static string AutoGenerateClientName(string clientId)
        {
            using (ClientDataContext db = new ClientDataContext())
            {
                var client = db.Clients.FirstOrDefault(c => c.Id == clientId);
                var rnd = new Random();
                client.Name = nicknames[rnd.Next(0, nicknames.Count - 1)];
                var clientState = db.ClientStates
                    .FirstOrDefault(c => c.ClientId == clientId);
                clientState.ClientStatus = Status.Default;
                db.SaveChanges();
                return $"Понимаю, вам неловко говорить своё имя...но все же я буду вас называть {client.Name}!)";
            }
        }

        public static string AddClientName(string clientId, string name)
        {
            using (ClientDataContext db = new ClientDataContext())
            {
                var client = db.Clients.FirstOrDefault(c => c.Id == clientId);
                client.Name = name;
                var clientState = db.ClientStates
                    .FirstOrDefault(c => c.ClientId == clientId);
                clientState.ClientStatus = Status.Default;
                db.SaveChanges();
                return "Отлично, рада знакомству!";
            }
        }

        public static string GetClientName(string clientId)
        {
            using (ClientDataContext db = new ClientDataContext())
                return db.Clients.FirstOrDefault(cl => cl.Id == clientId)?.Name;
        }

        public static string ChangeStateToAddStop(string clientId)
        {
            using (ClientDataContext db = new ClientDataContext())
            {
                var clientState = GetClientState(clientId);
                clientState.ClientStatus = Status.AddingStop;
                db.ClientStates.Update(clientState);
                db.SaveChanges();
                return clientState.GetInstruction();
            }
        }

        public static string AddBufferStop(string clientId, string stopName)
        {
            using (ClientDataContext db = new ClientDataContext())
            {
                var clientState = GetClientState(clientId);
                clientState.BufferStopName = stopName;
                clientState.ClientStatus = Status.AddingTag;
                db.ClientStates.Update(clientState);
                db.SaveChanges();
                return clientState.GetInstruction();
            }
        }

        public static string AddBufferTag(string clientId, string tagName)
        {
            using (ClientDataContext db = new ClientDataContext())
            {
                var clientState = GetClientState(clientId);
                clientState.BufferTagName = tagName;
                clientState.ClientStatus = Status.AddingRoute;
                db.ClientStates.Update(clientState);
                db.SaveChanges();
                return clientState.GetInstruction();
            }
        }

        public static string FindRouteDirections(string clientId, string routeName)
        {
            var route = Parser.FindRouteNum(routeName);
            if (route == null) return "Мне не удалось найти такой маршрут, проверьте правильность введенного маршрута";
            var directions = Parser.GetRouteChoice(route);
            using (ClientDataContext db = new ClientDataContext())
            {
                var clientState = GetClientState(clientId);
                clientState.ClientStatus = Status.ChoosingDirection;
                clientState.BufferRouteName = routeName;
                clientState.BufferDirections = directions;
                db.ClientStates.Update(clientState);
                db.SaveChanges();
                return clientState.GetInstruction();
            }
        }

        public static string AddStop(string clientId, string direction)
        {
            var client = GetClientState(clientId);
            var directionUrl = (direction == "1" || direction == "первое")
                ? client.BufferDirections.First().Value
                : client.BufferDirections.Last().Value;
            var stopLink = Parser.GetStop(directionUrl, client.BufferStopName);
            var stopUri = new Uri(stopLink);
            var stopId = HttpUtility.ParseQueryString(stopUri.Query).Get("st_id");
            var stop = new Stop(stopId, client.BufferStopName, stopLink);
            stop.Routes.Add(client.BufferRouteName);
            using (ClientDataContext db = new ClientDataContext())
            {
                var clientState = GetClientState(clientId);
                if (!db.Stops.Contains(stop))
                    db.Stops.Add(stop);
                db.ClientTags.Add(new ClientTag(clientId, client.BufferTagName, stopId));
                clientState.ClientStatus = Status.Default;
                db.ClientStates.Update(clientState);
                db.SaveChanges();
            }
            var timeIntervals = Parser.GetTime(stop);
            string result = $"Я добавила эту остановку по тегу {client.BufferTagName}. " +
                $"А вот и заодно время:\n";
            foreach (var time in timeIntervals)
                result += $"{time.Key}: {string.Join("\n", time.Value)}";
            return result;
        }

        public static string GetTimeByTag(string clientId, string tag)
        {
            using (ClientDataContext db = new ClientDataContext())
            {
                var stopId = db.ClientTags
                    .Where(c => c.ClientId == clientId)
                    .Where(c => c.TagName.Contains(tag))
                    .FirstOrDefault()?.StopId;
                if (stopId == null) return "Не удалось найти остановку по такому тегу";
                var stop = db.Stops.Where(s => s.Id == stopId).FirstOrDefault();
                var result = "Вот ваше время:\n";
                var timeIntervals = Parser.GetTime(stop);
                foreach (var time in timeIntervals)
                    result += $"{time.Key}: {string.Join("\n  ", time.Value)}\n";
                return result;
            }
        }
    }
}
