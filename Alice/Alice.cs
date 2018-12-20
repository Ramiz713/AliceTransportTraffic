using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using Microsoft.AspNetCore.Builder;
using TrafficTimetable;
using TrafficTimetable.Infrastructure;
using System.Text.RegularExpressions;

namespace Alice
{
    public class Alice : Controller
    {
        private static List<string> tags = new List<string> { "дом", "работа", "учёба" };
        private static Regex wordRegex = new Regex("[А-Я][а-яА-Я][^#&<>\"~;$^%{}?]{1,20}");
        private static Regex helloRegex = new Regex("привет|хай|как делишки|даров|здарова", RegexOptions.IgnoreCase);
        private static Regex stopRegex = new Regex("хочу добавить остановку", RegexOptions.IgnoreCase);
        private static Regex numberRegex = new Regex(@"[\d]|[\d][\d]|[\d][\d][\D]");
        private static Regex tagRegex = new Regex(@"дом|работ[а-я]|учёб[а-я]");


        private static string greeting = "Привет! Этот навык может быть полезен для быстрого получения информации о времени прибытия транспорта к остановке";

        static void Main(string[] args)
        {
            if (tagRegex.Match("работа дом").Success)
            {
                var result = tagRegex.Match("работа дом");
                    Console.WriteLine(result.Value);
            }
            Console.ReadKey();
            //CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
          WebHost.CreateDefaultBuilder(args)
            .ConfigureServices(srv => srv.AddCors().AddMvc())
            .Configure(app => app.UseCors(options => options.AllowAnyOrigin()
                                                            .AllowAnyMethod()
                                                            .AllowAnyHeader()
                                                            .AllowCredentials()).UseMvc());

        //static Replies _replies = new Replies
        //{
        //    ["привет хай здравствуй здравствуйте алиса"] = x => x.Reply($"Привет{FindClient(x.Session.UserId)}"),
        //    ["Салям"] = x => x.Reply("Абау, сез дә татарча беләсез әллә? Әфәрин!"),
        //    [""] = x => x.Reply()
        //    ["покажи список"] = x => x.Reply(Repository.ShowClientBusStops(x.Session.UserId)),
        //    ["*"] = x => x.Reply(Repository.FindBusCountOnRoute(x.Request.Command)),
        //    ["добавь"] = x => x.Reply(Repository.AddBusStop(x.Request.Command.Substring(7), x.Session.UserId), false, new ButtonModel[] { new ButtonModel { Title = "Показать", Hide = true, Url = "покажи" } })
        //};

        [HttpPost("/alice")]
        public AliceResponse WebHook([FromBody] AliceRequest req)
        {
            var command = req.Request.Command;
            var clientId = req.Session.UserId;
            if (command == "1" || command == "2")
                throw new NotImplementedException();
            if (helloRegex.Match(command).Success)
                return req.Reply($"Привет{FindClient(clientId)}");
            if (tagRegex.Match(command).Success)
            {
                var tag = tagRegex.Match(command).Value;
                return req.Reply(Repository.AddBufferTag(clientId, tag));
            }
            if (stopRegex.Match(command).Success)
                return req.Reply("Назовите название остановки");
            if (wordRegex.Match(command).Success)
            {
                if (Repository.IsClientExist(clientId))
                    return req.Reply(Repository.AddBufferStop(clientId, command));
                else
                    return req.Reply(Repository.AddClient(req.Session.UserId, command));
            }
            if (numberRegex.Match(command).Success)
                return req.Reply(Repository.FindRouteDirections(clientId, command),
                    buttons: new ButtonModel[]
                    {
                    new ButtonModel() { Title = "1", Hide = true },
                    new ButtonModel() { Title = "2", Hide = true }
                    });
            return req.Reply("Ничего");
        }

        private static string FindClient(string clientId)
        {
            var clientName = Repository.GetClientName(clientId);
            return (clientName == null)
                ? "! Кажется, я вас вижу, ой, слышу впервые... давайте знакомиться! Как вас зовут?"
                : $", {clientName}!";
        }
    }
}
