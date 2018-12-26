using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using TrafficTimetable.Domain;

namespace TrafficTimetable.Infrastructure
{
    internal static class Repository
    {
        private static List<string> nicknames = new List<string> { "Кабанчик","Зарубежный алмаз", "Рыночный клуб",
            "Первичный череп", "Философский Женя","Сложный пришелец", "Хитрый татарин" };

        private static string greeting = "Привет! Этот навык может быть полезен для быстрого получения информации о времени прибытия транспорта к остановке. Не хотите добавить остановку?";

        public static Response GetStateInfo(ClientState clientState)
        {
            string stateInfo = null;
            switch (clientState.ClientStatus)
            {
                case Status.AddingName:
                    stateInfo = "хотели сообщить мне своё имя.";
                    break;
                case Status.AddingStop:
                    stateInfo = "хотели назвать мне название остановки.";
                    break;
                case Status.AddingTag:
                    stateInfo = $"хотели назвать тег для остановки {clientState.BufferStopName}, которую вы добавляли.";
                    break;
                case Status.AddingRoute:
                    stateInfo = $"хотели добавить маршрут к названной вами остановке {clientState.BufferStopName}.";
                    break;
                case Status.ChoosingDirection:
                    stateInfo = $"хотели выбрать направление маршрута {clientState.BufferRouteName} {clientState.BufferTransportType} у остановки {clientState.BufferStopName}.";
                    break;
                default: return null;
            }
            return new Response($"Кажется, что наша работа была прервана. Вы {stateInfo} Хотите продолжить?",
                                new string[2] { "Да", "Нет" });
        }

        public static Response GetInstruction(ClientState clientState)
        {
            switch (clientState.ClientStatus)
            {
                case Status.AddingName:
                    return new Response("Как вас зовут?");
                case Status.AddingStop:
                    return new Response("Назовите название остановки");
                case Status.AddingTag:
                    return new Response("Назовите тег, который хотите привязать к этой остановке");
                case Status.AddingRoute:
                    return new Response("Назовите маршрут, время прибытия которого хотите узнать");
                case Status.ChoosingDirection:
                    return new Response($"Какое из направлений?\n 1. {clientState.BufferDirections.First().Key}\n " +
                        $"2.{clientState.BufferDirections.Last().Key}", new string[2] { "1", "2" });
                default:
                    return null;
            }
        }

        public static Response ShowSavedStops(string clientId)
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

        public static Response ReturnDafaultState(string clientId)
        {
            using (ClientDataContext db = new ClientDataContext())
            {
                var clientState = GetClientState(clientId);
                clientState.ClientStatus = Status.Default;
                db.ClientStates.Update(clientState);
                db.SaveChanges();
                return new Response("Сказано - не сделано!");
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
                return GetStateInfo(clientState);
            }
        }

        public static Response ContinueWorkOrChangeToDefaultState(string clientId, bool flag)
        {
            using (ClientDataContext db = new ClientDataContext())
            {
                var clientState = GetClientState(clientId);
                var response = GetInstruction(clientState);
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
                return $"! Кажется, я вас вижу, ой, слышу впервые... давайте знакомиться! {GetInstruction(clientState)}";
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
                return GetInstruction(clientState);
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
                return GetInstruction(clientState);
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
                return GetInstruction(clientState);
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
                return GetInstruction(clientState);
            }
        }

        public static Response AddStop(string clientId, string direction)
        {
            var client = GetClientState(clientId);
            var directionUrl = (direction == "1" || direction == "первое")
                ? client.BufferDirections.First().Value
                : client.BufferDirections.Last().Value;
            var stopLink = Parser.GetStop(directionUrl, client.BufferStopName);
            if (stopLink == null)
                return new Response("Я не смогла найти остановку с таким названием по указанному вами маршруту.");
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
            return new Response($"{result}\n");
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

        public static string AddRouteToTag(string clientId, string route, string tag)
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
