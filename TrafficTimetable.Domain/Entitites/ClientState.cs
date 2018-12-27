using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;


namespace TrafficTimetable.Domain
{
    public class ClientState
    {
        public ClientState(string clientId, string sessionId)
        {
            ClientId = clientId;
            SessionId = sessionId;
            ClientStatus = Status.Default;
        }

        [Key]
        public string ClientId { get; set; }

        public Client Client { get; set; }

        public string SessionId { get; set; }

        public Status ClientStatus { get; set; }
        public bool WaitingToContinue { get; set; }
        public string BufferDirection { get; set; }
        public string BufferStopName { get; set; }
        public string BufferRouteName { get; set; }
        public string BufferTagName { get; set; }

        [NotMapped]
        public Dictionary<string, string> BufferDirections
        {
            get { return JsonConvert.DeserializeObject<Dictionary<string, string>>(BufferDirection); }
            set { BufferDirection = JsonConvert.SerializeObject(value); }
        }
    }
}
