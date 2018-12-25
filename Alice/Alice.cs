﻿using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace Alice
{
    public class Response
    {
        public string Text { get; set; }
        public string[] Buttons { get; set; }
    }

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
            //обращаемся к серверу, а не к проекту с расписанием

            HttpWebRequest request = WebRequest.Create(
                        $"http://localhost:1234/timetable?userid={req.Session.UserId}&sessionid={req.Session.SessionId}&command={req.Request.Command}") 
                        as HttpWebRequest;

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            string responseString;

            using (var reader = new System.IO.StreamReader(response.GetResponseStream(), Encoding.ASCII))
            {
                responseString = reader.ReadToEnd();
            }
            byte[] encodedBytes = Encoding.Unicode.GetBytes(responseString);
           // encodedBytes = Encoding.Convert(Encoding.UTF8, Encoding.Unicode, encodedBytes);
            string result = System.Text.Encoding.UTF8.GetString(encodedBytes);

            var responseModel = JsonConvert.DeserializeObject<Response>(responseString);

            if (responseModel.Buttons.Length > 0)
                return req.Reply(responseModel.Text, buttons: CreateButtons(responseModel.Buttons));
            return req.Reply(responseModel.Text);
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
