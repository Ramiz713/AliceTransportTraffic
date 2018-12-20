using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using TrafficTimetable.Domain;

namespace TrafficTimetable.Infrastructure
{
    public static class Repository
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

        //public static string AddStop(string clientId, string direction)
        //{
        //    var client = GetClient(clientId);
        //    var directionUrl = (direction == "1")
        //        ? client.Directions.First().Key
        //        : client.Directions.Last().Key;
        //    var stopLink = Parser.GetStop(directionUrl, client.BufferStopName);
        //    var time = Parser.GetTime(stopLink)
        //}

        public static string FindRouteDirections(string clientId, string routeName)
        {
            var client = GetClient(clientId);
            client.BufferRouteName = routeName;
            var route = Parser.FindRouteNum(routeName);
            if (route == null) return "Мне не удалось найти такой маршрут, проверьте правильность введенного маршрута";
            var directions = Parser.GetRouteChoice(route);
            GetClient(clientId).Directions = directions;
            return $"Какое из направлений?\n 1. {directions.First().Value}\n 2.{directions.Last().Value}";
        }
    }
}
