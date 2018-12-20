using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using TrafficTimetable.Domain;

namespace TrafficTimetable.Infrastructure
{
    public static class Parser
    {
        private static string linkPattern = "http://navi.kazantransport.ru/old-site/wap/online/";

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

        public static Dictionary<string, string[]> GetTime(Stop stop)
        {
            var document = ParseUrl(stop.Url);
            var data = document.QuerySelectorAll("a").Where(x => x.TextContent != ">>").ToArray();
            var items = new Dictionary<string, string[]>();
            for (int i = 0; i < data.Take(data.Count() - 6).Count(); i += 4)
            {
                var route = data[i].TextContent;
                if (!(stop.Routes.Contains(route))) continue;
                var stopName = data[i + 1].TextContent;
                Regex checktime = new Regex("^(?:0?[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$");
                string firstTime = null;
                string secondTime = null;
                firstTime = data[i + 2].TextContent;
                if (checktime.IsMatch(data[i + 3].TextContent))
                    secondTime = data[i + 3].TextContent;
                else
                    i--;
                items[route] = new string[2] { firstTime, secondTime };
            }
            return items;
        }

        public static string FindRouteNum(string routeNum)
        {
            var document = ParseUrl("http://navi.kazantransport.ru/old-site/wap/online/?tt_id=1");
            var routeData = document
                .QuerySelectorAll("a")
                .Where(x => x.TextContent == routeNum)
                .OfType<IHtmlAnchorElement>()
                .ToList();
            try
            {
                return linkPattern + routeData.Last().Href.Remove(0, 9);
            }
            catch
            {
                return null;
            }
        }

        public static Dictionary<string, string> GetRouteChoice(string url)
        {
            var document = ParseUrl(url);
            var routes = new Dictionary<string, string>();
            var data = document
                .QuerySelectorAll("a")
                .OfType<IHtmlAnchorElement>()
                .Skip(1).ToList();
            foreach (var item in data.Take(data.Count - 8))//поставил 8, т.к актуальны только первые два направления
                routes.Add(item.Text, linkPattern + item.Href.Remove(0, 9));
            return routes;
        }

        public static string GetStop(string url, string stop)
        {
            var document = ParseUrl(url);
            var data = document
                .QuerySelectorAll("a")
                .OfType<IHtmlAnchorElement>()
                .Where(x => x.TextContent == stop);
            return linkPattern + data.First().Href.Remove(0, 9);
        }
    }
}
