using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using TrafficTimetable.Infrastructure;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Alice
{
    public class Alice : Controller
    {
        private static string greeting = "Привет! Этот навык может быть полезен для быстрого получения информации о времени прибытия транспорта к остановке";
        private static string firstMeeting = "! Кажется, я вас вижу, ой, слышу впервые... давайте знакомиться! Как вас зовут?";

        private static List<string> tags = new List<string> { "дом", "работа", "учёба" };
        private static Regex wordRegex = new Regex("[А-Я][а-яА-Я][^#&<>\"~;$^%{}?]{1,20}");
        private static Regex helloRegex = new Regex("привет|хай|как делишки|даров|здарова", RegexOptions.IgnoreCase);
        private static Regex stopRegex = new Regex("хочу добавить остановку", RegexOptions.IgnoreCase);
        private static Regex numberRegex = new Regex(@"[\d]|[\d][\d]|[\d][\d][\D]");
        private static Regex tagRegex = new Regex(@"дом|работ[а-я]|учёб[а-я]");

        private static ButtonModel[] directionButtons = new ButtonModel[]
        {
            new ButtonModel() { Title = "1", Hide = true },
            new ButtonModel() { Title = "2", Hide = true }
        };

        static void Main(string[] args) => CreateWebHostBuilder(args).Build().Run();

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
            var response = Handler.Handle(req.Session.UserId, req.Session.SessionId, req.Request.Command);
            if (response.Item2.Length > 0)
                return req.Reply(response.Item1, buttons: CreateButtons(response.Item2));
            return req.Reply(response.Item1);
        }

        private static ButtonModel[] CreateButtons(string[] values)
        {
            var listOfButtons = new List<ButtonModel>();
            foreach (var val in values)
                listOfButtons.Add(new ButtonModel { Title = val, Hide = true });
            return listOfButtons.ToArray();
        }
    }
}
