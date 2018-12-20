using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using TrafficTimetable.Domain;

namespace TrafficTimetable.Infrastructure
{
    public static class Repository
    {
        public static void Main()
        {
<<<<<<< HEAD
            var directions = Parser.GetDirectionsForRoute("4"); 
            //может быть пустой словарь
            //тогда это означает что нет маршрута с таким номером 
            //сообщи об этом пользователю
      
           
            
            // здесь для примера вывожу словарь направление и ссылку на него
            // нужно выбрать направление и передать дальше ссылку
            // ключ -  направление
            // значение ссылка
            foreach (var d in directions)
                Console.WriteLine(d.Key + " " + d.Value);
            Console.WriteLine(Parser.GetTime(/*ссылка допустим первый элемент из словаря*/directions.ElementAt(0).Value, "РКБ",/*указать номер маршрута*/ "4"));    
        }

        public static string AddBusStop(string busStop, string clientId)
=======
            using (ClientDataContext db = new ClientDataContext())
            {
                var clients = db.Clients;
                foreach (var client in clients)
                    Console.WriteLine(client.Id);
            }
            Console.ReadKey();
        }

        public static string AddClient(string clientId, string name)
>>>>>>> 7b5edad71a432c07cc9ca9dcfb1a694c8b8be063
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
<<<<<<< HEAD
            {
                var busStops = db.Clients.ToList().Where(c => c.Id == clientId).First().BusStops;
                return (busStops.Count() == 0)
                    ? "Empty. Client didn't add any bus stop"
                    : string.Join("\n", busStops);
            }
        }

        public static string FindBusCountOnRoute(string busRoute)
=======
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
>>>>>>> 7b5edad71a432c07cc9ca9dcfb1a694c8b8be063
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