using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using TrafficTimetable.Domain;

namespace TrafficTimetable.Infrastructure
{
    public class Stop
    {
        public string StopNum { get; set; }
        public string Name { get; set; }
        public string FirstTime { get; set; }
        public string SecondTime { get; set; }

        public Stop(string stopNum, string name, string time1, string time2)
        {
            StopNum = stopNum;
            Name = name;
            FirstTime = time1;
            SecondTime = time2;
        }

        public override string ToString()
        {
            return
                this.StopNum + "\t" + Name + "\t" + FirstTime + "\t" + SecondTime;
        }
    }
    public static class Parser
    {
        public static List<Stop> Parse(string url)
        {
            var parser = new HtmlParser();
            string htmlCode;
            //using (WebClient wb = new WebClient())
            //{
            //    htmlCode = wb.DownloadString("http://navi.kazantransport.ru/old-site/wap/online/?st_id=81");
            //}
            using (StreamReader sr = new StreamReader("..\\TextFile1.txt", Encoding.UTF8))
            {
                htmlCode = sr.ReadToEnd();
            }
            var document = parser.Parse(htmlCode);
            var smth = document.QuerySelectorAll("a").Where(x => x.TextContent != ">>").ToArray();
            var items = new List<Stop>();
            for (int i =0; i<smth.Take(smth.Count()-6).Count(); i+=4)
            {
                items.Add(new Stop(smth[i].TextContent, smth[i + 1].TextContent, smth[i + 2].TextContent, smth[i + 3].TextContent));
            }
            return items;

        }
    }
    public static class Repository
    {
        public static void Main()
        {
            //using (ClientDataContext db = new ClientDataContext())
            //{
            //    var clients = db.Clients.ToList();
            //    foreach (var client in clients)
            //        Console.WriteLine(client.Id);
            //}
            //Console.ReadKey();
            string url = "здесь будет url";
            var smth = Parser.Parse(url);
            foreach (var item in smth)
                Console.WriteLine(item.ToString());
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