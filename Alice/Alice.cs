using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using TrafficTimetable.Infrastructure;

namespace Alice
{
    public class Alice : Controller
    {
        private static string greeting = "Привет! Этот навык может быть полезен для быстрого получения информации о времени прибытия транспорта к остановке";

        static void Main(string[] args)
        {
            //if (tagRegex.Match("работа дом").Success)
            //{
            //    var result = tagRegex.Match("работа дом");
            //    Console.WriteLine(result.Value);
            //}
            //Console.ReadKey();
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
          WebHost.CreateDefaultBuilder(args)
            .ConfigureServices(srv => srv.AddCors().AddMvc())
            .Configure(app => app.UseCors(options => options.AllowAnyOrigin()
                                                            .AllowAnyMethod()
                                                            .AllowAnyHeader()
                                                            .AllowCredentials()).UseMvc());

        [HttpPost("/alice")]
        public AliceResponse WebHook([FromBody] AliceRequest req)
        {
            req.Reply(Handler.Handle(req.Session.UserId, req.Session.SessionId, req.Request.Command));
        }


    }
}
