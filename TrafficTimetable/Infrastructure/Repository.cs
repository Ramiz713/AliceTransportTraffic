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
        public static void Main()
        {
            using (ClientDataContext db = new ClientDataContext())
            {
                var clientStates = db.ClientStates;
                foreach (var client in clientStates)
                    Console.WriteLine(client.ClientId);
            }
            Console.ReadKey();
        }

        //public static string ShowSavedStops(string clientId)
        //{
        //    using (ClientDataContext db = new ClientDataContext())
        //    {
        //        var clientTags = db.ClientTags.Where(c => c.ClientId == clientId); cl
        //        var result = from cl in clientTags
        //                     join
        //    }
        //}

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

        public static string GetMessageInWhichStateUser(string clientId, string sessionId)
        {
            using (ClientDataContext db = new ClientDataContext())
            {
                var clientState = GetClientState(clientId);
                clientState.SessionId = sessionId;
                clientState.WaitingToContinue = true;
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

        public static string ChangeStateToAddStop(string clientId, string sessionId)
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

        public static string AddStop(string clientId, string sessionId, string direction)
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
            var times = Parser.GetTime(stop);
            string result = $"Я добавила эту остановку по тегу {client.BufferTagName}. " +
                $"А вот и заодно время:\n";
            foreach (var time in times)
                result += $"{time.Key}: {string.Join("\n", time.Value)}";
            return result;
        }
    }
}
