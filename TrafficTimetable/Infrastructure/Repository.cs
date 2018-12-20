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
                    Console.WriteLine(client.ClientId + client.IsAddName);
            }
            Console.ReadKey();
        }

        public static ClientState CreateClientState(string clientId, string sessionId)
        {
            using (ClientDataContext db = new ClientDataContext())
            {
                var clientState = new ClientState(clientId, sessionId);
                clientState.IsDefault = true;
                db.ClientStates.Add(clientState);
                db.SaveChanges();
                return clientState;
            }
        }

        public static ClientState GetClientState(string clientId, string sessionId)
        {
            using (ClientDataContext db = new ClientDataContext())
                return db.ClientStates
                    .FirstOrDefault(c => c.ClientId == clientId && c.SessionId == sessionId);
        }

        public static string ChangeStateToAddNameAndAddClient(string clientId, string sessionId)
        {
            using (ClientDataContext db = new ClientDataContext())
            {
                db.Clients.Add(new Client(clientId));
                var clientState = CreateClientState(clientId, sessionId);
                clientState.IsAddName = true;
                db.SaveChanges();
            }
            return "! Кажется, я вас вижу, ой, слышу впервые... давайте знакомиться! Как вас зовут?";
        }

        public static string ChangeStateToAddStop(string clientId)
        {
            using (ClientDataContext db = new ClientDataContext())
            {
                var clientState = db.ClientStates.FirstOrDefault(c => c.ClientId == clientId);
                clientState.IsDefault = false;
                clientState.IsAddStop = true;
                db.SaveChanges();
            }
            return "Назовите название остановки";
        }



        public static string AddClientName(string clientId, string name)
        {
            using (ClientDataContext db = new ClientDataContext())
            {
                var client = db.Clients.FirstOrDefault(c => c.Id == clientId);
                client.Name = name;
                var clientState = db.ClientStates.FirstOrDefault(c => c.ClientId == clientId);
                clientState.IsAddName = false;
                clientState.IsDefault = true;
                db.SaveChanges();
            }
            return "Отлично, рада знакомству!";
        }

        private static Client GetClient(string clientId)
        {
            using (ClientDataContext db = new ClientDataContext())
                return db.Clients.FirstOrDefault(cl => cl.Id == clientId);
        }

        public static string GetClientName(string clientId) =>
            GetClient(clientId)?.Name;

        public static bool IsClientExist(string clientId)
        {
            using (ClientDataContext db = new ClientDataContext())
                return db.Clients.FirstOrDefault(cl => cl.Id == clientId) != null;
        }

        public static string AddBufferStop(string clientId, string stopName)
        {
            using (ClientDataContext db = new ClientDataContext())
            {
                var clientState = db.ClientStates.FirstOrDefault(c => c.ClientId == clientId);
                clientState.IsAddStop = false;
                clientState.IsAddTag = true;
                clientState.BufferStopName = stopName;
                db.SaveChanges();
                return "Назовите тег, который хотите привязать к этой остановке";
            }
        }

        public static string AddBufferTag(string clientId, string tagName)
        {
            using (ClientDataContext db = new ClientDataContext())
            {
                var clientState = db.ClientStates.FirstOrDefault(c => c.ClientId == clientId);
                clientState.IsAddTag = false;
                clientState.IsAddRoute = true;
                clientState.BufferTagName = tagName;
                db.SaveChanges();
                return "Назовите маршрут, время прибытия которого хотите узнать";
            }
        }

        public static string AddStop(string clientId, string sessionId, string direction)
        {
            var client = GetClientState(clientId, sessionId);
            var directionUrl = (direction == "1")
                ? client.BufferDirections.First().Value
                : client.BufferDirections.Last().Value;
            var stopLink = Parser.GetStop(directionUrl, client.BufferStopName);
            var stopUri = new Uri(stopLink);
            var stopId = HttpUtility.ParseQueryString(stopUri.Query).Get("st_id");
            var stop = new Stop(stopId, client.BufferStopName, stopLink);
            stop.Routes.Add(client.BufferRouteName);
            using (ClientDataContext db = new ClientDataContext())
            {
                var clientState = db.ClientStates.FirstOrDefault(c => c.ClientId == clientId);
                if (!db.Stops.Contains(stop))
                    db.Stops.Add(stop);
                db.ClientTags.Add(new ClientTag(clientId, client.BufferTagName, stopId));
                clientState.ResetClientState();
                db.SaveChanges();
            }
            var times = Parser.GetTime(stop);
            string result =  $"Я добавила эту остановку по тегу {client.BufferTagName}. " +
                $"А вот и заодно время:\n";
            foreach (var time in times)
            {
                result += $"{time.Key}: {string.Join("\n", time.Value)}";
            }
            return result;
        }

        public static string FindRouteDirections(string clientId, string routeName)
        {
            var route = Parser.FindRouteNum(routeName);
            if (route == null) return "Мне не удалось найти такой маршрут, проверьте правильность введенного маршрута";
            var directions = Parser.GetRouteChoice(route);
            using (ClientDataContext db = new ClientDataContext())
            {
                var clientState = db.ClientStates.FirstOrDefault(c => c.ClientId == clientId);
                clientState.IsAddRoute = false;
                clientState.IsChoosingDirection = true;
                clientState.BufferRouteName = routeName;
                clientState.BufferDirections = directions;
                db.SaveChanges();
            }
            return $"Какое из направлений?\n 1. {directions.First().Key}\n 2.{directions.Last().Key}";
        }
    }
}
