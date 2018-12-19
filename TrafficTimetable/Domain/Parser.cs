using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace TrafficTimetable.Domain
{
    public class Stop
    {
        public readonly string route;
        private readonly string direction;
        private readonly string firstTime;
        private readonly string secondTime;

        public Stop(string route, string direction, string firstTime, string secondTime)
        {
            this.route = route;
            this.direction = direction;
            this.firstTime = firstTime;
            this.secondTime = secondTime;
        }

        public override string ToString()
        {
            return $"{route}\t${direction}\t{firstTime}\t{secondTime}";
        }
    }

    public static class Parser
    {
        private static readonly string noInfoMessage = "нет инфы о маршруте, который тебе нужен";

        private static readonly string linkPattern = "http://navi.kazantransport.ru/old-site/wap/online/";

        private static IHtmlDocument ParseUrl(string url)
        {
            var parser = new HtmlParser();
            string htmlCode;
            using (WebClient wb = new WebClient())
            {
                htmlCode = wb.DownloadString(url);
            }
            var document = parser.Parse(htmlCode);
            return document;
        }

        private static string FindTime(string url, string routeNum)
        {
            var document = ParseUrl(url);
            var data = document.QuerySelectorAll("a")
                .Where(x => x.TextContent != ">>")
                .ToArray();
            var items = new List<Stop>();
            for (int i = 0; i < data.Take(data.Count() - 6).Count(); i += 4)
            {
                var route = data[i].TextContent;
                var stopName = data[i + 1].TextContent;
                Regex checktime = new Regex("^(?:0?[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$");
                string ftime = null;
                string stime = null;
                ftime = data[i + 2].TextContent;
                if (checktime.IsMatch(data[i + 3].TextContent))
                {
                    stime = data[i + 3].TextContent;
                }
                else
                {
                    i--;
                }
                var stop = new Stop(route, stopName, ftime, stime);
                items.Add(stop);
            }
            var stops = items.Where(s => s.route == routeNum);
            if (stops.Count() == 0)
                return noInfoMessage;
            return stops.First().ToString(); 
        }

        private static string FindRouteNum(string routeNum)
        {
            var document = ParseUrl("http://navi.kazantransport.ru/old-site/wap/online/?tt_id=1");
            var routeData = document.QuerySelectorAll("a").Where(x => x.TextContent == routeNum).OfType<IHtmlAnchorElement>().ToList();
            try
            {
                return linkPattern + routeData.Last().Href.Remove(0, 9);
            }
            catch
            {
                return null;
            }
        }

        private static Dictionary<string, string> GetRouteChoice(string url)
        {
            var document = ParseUrl(url);
            var routes = new Dictionary<string, string>();
            var data = document.QuerySelectorAll("a").OfType<IHtmlAnchorElement>().Skip(1).ToList();
            foreach (var item in data.Take(data.Count - 6))
            {
                routes.Add(item.Text, linkPattern + item.Href.Remove(0, 9));
            }
            return routes;
        }

        private static string GetLinkForStop(string url, string stop)
        {
            var document = ParseUrl(url);
            var data = document.QuerySelectorAll("a").OfType<IHtmlAnchorElement>().Where(x => x.TextContent == stop);
            return linkPattern + data.First().Href.Remove(0, 9);
        }

        public static Dictionary<string, string> GetDirectionsForRoute(string route)
        {            
            var linkToRoute = FindRouteNum(route);
            if (linkToRoute == null)
                return new Dictionary<string, string>();
            return GetRouteChoice(linkToRoute);
        }

        public static string GetTime(string linkToDirection, string stopName, string routeNum)
        {
            var linkToTime = GetLinkForStop(linkToDirection, stopName);
            return FindTime(linkToTime, routeNum);
        }
    }
}
