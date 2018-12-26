using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using TrafficTimetable.Domain;
using TrafficTimetable.Infrastructure.Helpers;

namespace TrafficTimetable.Infrastructure
{
    internal static class Repository
    {
        private static List<string> nicknames = new List<string> { "Кабанчик","Зарубежный алмаз", "Рыночный клуб",
            "Первичный череп", "Философский Женя","Сложный пришелец", "Хитрый татарин" };

        private static string greeting = "Привет! Этот навык может быть полезен для быстрого получения информации о времени прибытия транспорта к остановке. Не хотите добавить остановку?";

        public static Response ShowSavedStops(string clientId)
        {
            using (ClientDataContext db = new ClientDataContext())
            {
                var clientStops = db.ClientTags.Where(c => c.ClientId == clientId)
                    .Join(db.Stops, tag => tag.StopId, stop => stop.Id,
                    (tag, stop) => new { tag.TagName, stop.Name, tag.Routes });
                if (!clientStops.Any()) return new Response("А здесь пусто. Пока что.");
                StringBuilder result = new StringBuilder().Append("Все ваши сохраненные остановки:\n");
                foreach (var stop in clientStops)
                    result
                        .Append("Тег остановки: ")
                        .Append(stop.TagName)
                        .Append("\n")
                        .Append("Остановка: ")
                        .Append(stop.Name)
                        .Append("\n")
                        .Append("Маршруты: ")
                        .Append(string.Join(", ", stop.Routes))
                        .Append("\n\n");
                return new Response(result.ToString());
            }
        }

        public static Response CreateClientAndState(string clientId, string sessionId)
        {
            using (ClientDataContext db = new ClientDataContext())
            {
                var clientState = new ClientState(clientId, sessionId);
                db.ClientStates.Add(clientState);
                db.Clients.Add(new Client(clientId));
                db.SaveChanges();
                return new Response(greeting, new string[1] { "хочу добавить остановку" });
            }
        }

        public static ClientState GetClientState(string clientId)
        {
            using (ClientDataContext db = new ClientDataContext())
                return db.ClientStates
                    .FirstOrDefault(c => c.ClientId == clientId);
        }

        public static Response ReturnDafaultState(string clientId, string text)
        {
            using (ClientDataContext db = new ClientDataContext())
            {
                var clientState = GetClientState(clientId);
                clientState.ClientStatus = Status.Default;
                db.ClientStates.Update(clientState);
                db.SaveChanges();
                return new Response(text);
            }
        }

        public static Response GetResponseUserState(string clientId, string newSessionId)
        {
            using (ClientDataContext db = new ClientDataContext())
            {
                var clientState = GetClientState(clientId);
                clientState.SessionId = newSessionId;
                if (clientState.ClientStatus != Status.Default) clientState.WaitingToContinue = true;
                db.ClientStates.Update(clientState);
                db.SaveChanges();
                return StateHelper.GetStateInfo(clientState);
            }
        }

        public static Response ContinueWorkOrChangeToDefaultState(string clientId, bool flag)
        {
            using (ClientDataContext db = new ClientDataContext())
            {
                var clientState = GetClientState(clientId);
                var response = StateHelper.GetInstruction(clientState);
                if (!flag)
                {
                    clientState.ClientStatus = Status.Default;

                    response = new Response(@"Хорошо, как говорится, забудем ""старое""");
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
                return $"! Кажется, я вас вижу, ой, слышу впервые... давайте знакомиться! Как вас зовут?";
            }
        }

        public static Response AutoGenerateClientName(string clientId)
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
                return new Response($"Понимаю, вам неловко говорить своё имя...но все же я буду вас называть {client.Name}!)");
            }
        }

        public static Response AddClientName(string clientId, string name)
        {
            using (ClientDataContext db = new ClientDataContext())
            {
                var client = db.Clients.FirstOrDefault(c => c.Id == clientId);
                client.Name = name;
                var clientState = db.ClientStates
                    .FirstOrDefault(c => c.ClientId == clientId);
                clientState.ClientStatus = Status.Default;
                db.SaveChanges();
                return new Response("Отлично, рада знакомству!");
            }
        }

        public static string GetClientName(string clientId)
        {
            using (ClientDataContext db = new ClientDataContext())
                return db.Clients.FirstOrDefault(cl => cl.Id == clientId)?.Name;
        }

        public static Response ChangeStateToAddStop(string clientId)
        {
            using (ClientDataContext db = new ClientDataContext())
            {
                var clientState = GetClientState(clientId);
                clientState.ClientStatus = Status.AddingStop;
                db.ClientStates.Update(clientState);
                db.SaveChanges();
                return StateHelper.GetInstruction(clientState);
            }
        }

        public static Response AddBufferStop(string clientId, string stopName)
        {
            using (ClientDataContext db = new ClientDataContext())
            {
                var clientState = GetClientState(clientId);
                clientState.BufferStopName = stopName;
                clientState.ClientStatus = Status.AddingTag;
                db.ClientStates.Update(clientState);
                db.SaveChanges();
                return StateHelper.GetInstruction(clientState);
            }
        }

        public static Response AddBufferTag(string clientId, string tagName)
        {
            using (ClientDataContext db = new ClientDataContext())
            {
                var clientState = GetClientState(clientId);
                clientState.BufferTagName = tagName;
                clientState.ClientStatus = Status.AddingRoute;
                db.ClientStates.Update(clientState);
                db.SaveChanges();
                return StateHelper.GetInstruction(clientState);
            }
        }

        public static Response FindRouteDirections(string clientId, string routeName)
        {
            var route = Parser.FindRouteNum(routeName);
            if (route == null) return new Response("Мне не удалось найти такой маршрут, проверьте правильность введенного маршрута");
            var directions = Parser.GetRouteChoice(route);
            using (ClientDataContext db = new ClientDataContext())
            {
                var clientState = GetClientState(clientId);
                clientState.ClientStatus = Status.ChoosingDirection;
                clientState.BufferRouteName = routeName;
                clientState.BufferDirections = directions;
                db.ClientStates.Update(clientState);
                db.SaveChanges();
                return StateHelper.GetInstruction(clientState);
            }
        }

        public static Response AddStop(string clientId, string direction)
        {
            var clientState = GetClientState(clientId);
            var directionUrl = (direction == "1" || direction == "первое")
                ? clientState.BufferDirections.First().Value
                : clientState.BufferDirections.Last().Value;
            var stopLink = Parser.GetStop(directionUrl, clientState.BufferStopName);
            if (stopLink == null)
                return ReturnDafaultState(clientId, "Я не смогла найти остановку с таким названием по указанному вами маршруту.");
            var stopUri = new Uri(stopLink);
            var stopId = HttpUtility.ParseQueryString(stopUri.Query).Get("st_id");
            Stop stop;
            using (ClientDataContext db = new ClientDataContext())
            {
                stop = db.Stops.Where(s => s.Id == stopId).FirstOrDefault();
                if (stop == null)
                {
                    stop = new Stop(stopId, clientState.BufferStopName, stopLink);
                    db.Stops.Add(stop);
                }
                if (db.ClientTags
                    .Where(c => c.TagName == clientState.BufferTagName && c.ClientId == clientId).FirstOrDefault() != null)
                    return ReturnDafaultState(clientId, $"У вас уже есть такая остановка с тэгом{clientState.BufferTagName}");
                var clientTag = new ClientTag(clientId, clientState.BufferTagName, stopId);
                clientTag.Routes.Add(clientState.BufferRouteName);
                db.ClientTags.Add(clientTag);
                clientState.ClientStatus = Status.Default;
                db.ClientStates.Update(clientState);
                db.SaveChanges();
            }
            var timeIntervals = Parser.GetTime(stop, new List<string> { clientState.BufferRouteName });
            string result = $"Я добавила эту остановку по тегу {clientState.BufferTagName}. " +
                "А вот и заодно время прибытия транспорта:\n";
            result += $"{string.Join("\n", timeIntervals[clientState.BufferRouteName])}";
            return new Response($"{result}\n");
        }

        public static Response GetTimeByTag(string clientId, string tag)
        {
            using (ClientDataContext db = new ClientDataContext())
            {
                var clientTag = db.ClientTags
                    .Where(c => c.ClientId == clientId)
                    .Where(c => c.TagName.Contains(tag))
                    .FirstOrDefault();
                if (clientTag == null) return new Response("Не удалось найти остановку по такому тегу");
                var stop = db.Stops.Where(s => s.Id == clientTag.StopId).FirstOrDefault();
                var result = "Вот ваше время:\n";
                var timeIntervals = Parser.GetTime(stop, clientTag.Routes);
                foreach (var time in timeIntervals)
                    result += $"{time.Key}: {string.Join("\n  ", time.Value)}\n";
                return new Response(result);
            }
        }

        public static Response AddRouteToTag(string clientId, string tag, string route)
        {
            using (ClientDataContext db = new ClientDataContext())
            {
                var clientTag = db.ClientTags
                    .Where(c => c.ClientId == clientId)
                    .Where(c => c.TagName.Contains(tag))
                    .FirstOrDefault();
                if (clientTag == null) return new Response("Не удалось найти остановку по такому тегу");
                var stop = db.Stops.Where(s => s.Id == clientTag.StopId).FirstOrDefault();
                var timeIntervals = Parser.GetTime(stop, new List<string> { route });
                if (!timeIntervals.Any())
                    return new Response("Не удалось добавить маршрут, проверьте корректность названия маршрута");
                var routes = clientTag.Routes;
                if (routes.Contains(route))
                    return new Response("Маршрут уже был добавлен ранее");
                clientTag.Routes.Add(route);
                db.ClientTags.Update(clientTag);
                db.SaveChanges();
                return new Response("Добавлено!");
            }
        }
    }
}
