using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TrafficTimetable.Infrastructure;

namespace Listener
{
    class Program
    {
        static void Main(string[] args)
        {
            Listen().Wait();
        }

        private static async Task Listen()
        {
            var listener = new HttpListener();
            listener.Prefixes.Add(@"http://localhost:1234/timetable/");
            listener.Start();
            while (true)
            {
                HttpListenerContext context = await listener.GetContextAsync();
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;
                string responseString;
                if (request.QueryString.AllKeys.Contains("userid") && request.QueryString.AllKeys.Contains("sessionid") && request.QueryString.AllKeys.Contains("command"))
                {
                    responseString = GetTimetable(request.QueryString["userid"], request.QueryString["sessionid"], request.QueryString["command"]);
                }
                else
                {
                    responseString = "Can't find what you are looking for.";
                }
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                response.ContentLength64 = buffer.Length;
                using (Stream output = response.OutputStream)
                {
                    output.Write(buffer, 0, buffer.Length);
                }
            }
            //listener.Stop();
        }

        private static string GetTimetable(string userId, string sessionId, string command)
        {
            var timetable = Handler.Handle(userId, sessionId, command);
            var responseModel = new ResponseModel { Text = timetable.Item1, Buttons = timetable.Item2 };
            return JsonConvert.SerializeObject(responseModel);
        }

        private class ResponseModel
        {
            [JsonProperty("Text")]
            internal string Text { get; set; }

            [JsonProperty("Buttons")]
            internal string[] Buttons { get; set; }
        }
    }
}
