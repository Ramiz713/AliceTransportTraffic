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
        {   //нет маршрута с таким номером
            var routeException = Parser.FindRouteNum("3");
            Console.WriteLine(routeException);
            // ищем автобусный маршрут с номером 4
            var route = Parser.FindRouteNum("4");
            Console.WriteLine(route);
            // получили ссылку
            var directions = Parser.GetRouteChoice(route);

            //
            //можно конечно было бы инкапсулировать направления и маршрут в один метод но там будет труднее с пробрасыванием исключения
            //
            
            // здесь для примера вывожу словарь направление и ссылку на него
            // нужно выбрать направление и передать дальше ссылку
            foreach (var d in directions)
                Console.WriteLine(d.Key + " " + d.Value);
            var linkToTime = Parser.GetStop(/*ссылка допустим первый элемент из словаря*/directions.ElementAt(0).Value, "РКБ");
            Console.WriteLine(linkToTime);
            var time = Parser.GetTime(linkToTime).Where(t=>t.route == "4").First();
            //чтоб не выводить лишние номера маршрутов просто фильтруем все маршруты
            Console.WriteLine(time.ToString());
            
        }

        public static string AddBusStop(string busStop, string clientId)
        {
            using (ClientDataContext db = new ClientDataContext())
            {
                var clients = db.Clients.ToList().Where(c => c.Id == clientId);
                Client client;
                if (clients.Count() == 0)
                {
                    client = new Client { Id = clientId, BusStops = new List<string>() };
                    db.Clients.Add(client);
                }
                else client = clients.First();
                client.BusStops.Add(busStop);
                db.SaveChanges();
                return "Added";
            }
        }

        public static string ClearBusStops(string clientId)
        {
            using (ClientDataContext db = new ClientDataContext())
            {
                var clients = db.Clients.ToList().Where(c => c.Id == clientId);
                Client client;
                if (clients.Count() == 0)
                {
                    return "You haven't bus stops";
                }
                else client = clients.First();
                client.BusStops.Clear();
                db.SaveChanges();
                return "Cleared";
            }
        }

        public static string ShowClientBusStops(string clientId)
        {
            using (ClientDataContext db = new ClientDataContext())
            {
                var busStops = db.Clients.ToList().Where(c => c.Id == clientId).First().BusStops;
                return (busStops.Count() == 0)
                    ? "Empty. Client didn't add any bus stop"
                    : string.Join("\n", busStops);
            }
        }

        public static string FindBusCountOnRoute(string busRoute)
        {
            WebClient wc = new WebClient();
            var json = wc.DownloadString(@"http://data.kzn.ru:8082/api/v0/dynamic_datasets/bus.json");
            var buses = JsonConvert.DeserializeObject<List<BusTime>>(json);
            return buses.Where(bus => bus.Bus.Route == busRoute).Count().ToString();
        }
    }
}