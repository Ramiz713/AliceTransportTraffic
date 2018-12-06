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
                var clients = db.Clients.ToList();
                foreach (var client in clients)
                    Console.WriteLine(client.Id);
            }
            Console.ReadKey();
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
                var clients = db.Clients.ToList().Where(c => c.Id == clientId);
                return (clients.Count() == 0)
                    ? "Empty. Client didn't add any bus stop"
                    : string.Join("\n", clients.First().BusStops);
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
