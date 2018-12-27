using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Alice
{
    public static class Extensions
    {
        public static AliceResponse Reply(
          this AliceRequest req,
          string text,
          bool endSession = false,
          ButtonModel[] buttons = null)
        {
            var resp = new AliceResponse
            {
                Response = new ResponseModel
                {
                    Text = text,
                    Tts = text,
                    EndSession = endSession
                },
                Session = req.Session
            };
            resp.Response.Buttons = buttons;
            return resp;
        }
    }
}
