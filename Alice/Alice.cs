using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using Microsoft.AspNetCore.Builder;
using TrafficTimetable.Infrastructure;

namespace Alice
{
    public class Alice : Controller
    {
        static void Main(string[] args) => CreateWebHostBuilder(args).Build().Run();

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
          WebHost.CreateDefaultBuilder(args)
            .ConfigureServices(srv => srv.AddCors().AddMvc())
            .Configure(app => app.UseCors(options => options.AllowAnyOrigin()
                                                            .AllowAnyMethod()
                                                            .AllowAnyHeader()
                                                            .AllowCredentials()).UseMvc());
        static Replies _replies = new Replies
        {
            ["привет хай здравствуй здравствуйте алиса"] = x => x.Reply("Привет"),
            ["Сәлам Исәнмесез Хәлләр ничек"] = x => x.Reply("Абау, сез дә татарча беләсез мәллә? Әфәрин!"),
            ["удали почисти"] = x => x.Reply(Repository.ClearBusStops(x.Session.UserId)),
            ["покажи список"] = x => x.Reply(Repository.ShowClientBusStops(x.Session.UserId)),
            ["добавь"] = x => x.Reply(Repository.AddBusStop(x.Request.Command.Substring(7), x.Session.UserId)),
            ["*"] = x => x.Reply("Даже не знаю, что вам ответить")
        };

        [HttpPost("/alice")]
        public AliceResponse WebHook([FromBody] AliceRequest req) => _replies.Match(req);
    }
}
