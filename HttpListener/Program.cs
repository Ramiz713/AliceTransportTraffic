using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TrafficTimetable.Infrastructure;

namespace Listener
{
    class Program
    {
        static void Main(string[] args)
        {
            Listen();
        }

        private static void Listen()
        {
            var listener = new HttpListener();
            listener.Prefixes.Add(@"http://localhost:1234/timetable/");
            listener.Start();
            while (true)
            {
                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;
                Task.Run(() =>
                {
                    string responseString;
                    if (request.QueryString.AllKeys.Contains("userid") && request.QueryString.AllKeys.Contains("sessionid") && request.QueryString.AllKeys.Contains("command"))
                    {
                        responseString = GetTimetable(request.QueryString["userid"], request.QueryString["sessionid"], request.QueryString["command"]);
                    }
                    else
                    {
                        responseString = "Can't find what you are looking for.";
                    }
                    byte[] buffer = Encoding.UTF8.GetBytes(responseString);
                    response.ContentLength64 = buffer.Length;
                    using (Stream output = response.OutputStream)
                    {
                        output.Write(buffer, 0, buffer.Length);
                    }
                });
            }
        }

        private static string GetTimetable(string userId, string sessionId, string command)
        {
            var timetable = Handler.Handle(userId, sessionId, command);
            var responseModel = JsonConvert.SerializeObject(timetable);
            return responseModel;
        }
    }
}
