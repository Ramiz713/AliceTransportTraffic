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
                var clients = db.Clients;
                foreach (var client in clients)
                    Console.WriteLine(client.Id);
            }
            Console.ReadKey();
        }

        public static string AddClient(string clientId, string name)
        {
            using (ClientDataContext db = new ClientDataContext())
            {
                db.Clients.Add(new Client(clientId, name));
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
            var client = GetClient(clientId);
            client.BufferStopName = stopName;
            return "Назовите тэг, который хотите привязать к этой остановке";
        }

        public static string AddBufferTag(string clientId, string tagName)
        {
            var client = GetClient(clientId);
            client.BufferTagName = tagName;
            return "Назовите маршрут, время прибытия которого хотите узнать";
        }

        public static string AddStop(string clientId, string direction)
        {
            var client = GetClient(clientId);
            var directionUrl = (direction == "1")
                ? client.BufferDirections.First().Value
                : client.BufferDirections.Last().Value;
            var stopLink = Parser.GetStop(directionUrl, client.BufferStopName);
            var stopUri = new Uri(stopLink);
            var stopId = HttpUtility.ParseQueryString(stopUri.Query).Get("st_id");
            var stop = new Stop(stopId, client.BufferStopName, stopLink);
            using (ClientDataContext db = new ClientDataContext())
            {
                db.Stops.Add(stop);
                db.ClientTags.Add(new ClientTag(clientId, client.BufferTagName, stopId));
                db.SaveChanges();
            }
            var times = Parser.GetTime(stop);
            string result = "";
            foreach(var time in times)
            {
                result += $"{time.Key}: {string.Join("\n", time.Value)}";
            }
            return result;
        }

        public static string FindRouteDirections(string clientId, string routeName)
        {
            var client = GetClient(clientId);
            client.BufferRouteName = routeName;
            var route = Parser.FindRouteNum(routeName);
            if (route == null) return "Мне не удалось найти такой маршрут, проверьте правильность введенного маршрута";
            var directions = Parser.GetRouteChoice(route);
            GetClient(clientId).BufferDirections = directions;
            return $"Какое из направлений?\n 1. {directions.First().Key}\n 2.{directions.Last().Key}";
        }
    }
}
