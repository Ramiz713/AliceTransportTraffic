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
