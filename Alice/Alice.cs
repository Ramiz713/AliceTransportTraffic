﻿using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using System.Threading.Tasks;

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
            return GetResponse(req).Result;
        }

        private static ButtonModel[] CreateButtons(string[] values)
        {
            var listOfButtons = new List<ButtonModel>();
            foreach (var val in values)
                listOfButtons.Add(new ButtonModel { Title = val, Hide = true });
            return listOfButtons.ToArray();
        }

        private static async Task<AliceResponse> GetResponse(AliceRequest req)
        {
            return await Task.Run(() =>
            {
               HttpWebRequest request = WebRequest.Create(
                       $"http://localhost:1234/timetable?userid={req.Session.UserId}&sessionid={req.Session.SessionId}&command={req.Request.Command}")
                       as HttpWebRequest;

               HttpWebResponse response = (HttpWebResponse)request.GetResponse();

               string responseString;

               using (var reader = new System.IO.StreamReader(response.GetResponseStream(), Encoding.UTF8))
               {
                   responseString = reader.ReadToEnd();
               }

               var responseModel = JsonConvert.DeserializeObject<Response>(responseString);

               if (responseModel.Buttons?.Length > 0)
                   return req.Reply(responseModel.Text, buttons: CreateButtons(responseModel.Buttons));
               return req.Reply(responseModel.Text);
           });
        }
    }
}
