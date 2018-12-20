using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TrafficTimetable.Domain
{
    public class ClientState
    {
        public ClientState(string clientId, string sessionId)
        {
            ClientId = clientId;
            SessionId = sessionId;
        }

        [Key]
        public string ClientId { get; set; }
        [Key]
        public string SessionId { get; set; }

        public bool IsAddName { get; set; }
        public bool IsDefault { get; set; }
        public bool IsAddStop { get; set; }
        public bool IsAddRoute { get; set; }
        public bool IsAddTag { get; set; }
        public bool IsChoosingDirection { get; set; }
        public string BufferDirection { get; set; }
        public string BufferStopName { get; set; }
        public string BufferRouteName { get; set; }
        public string BufferTagName { get; set; }

        public void ResetClientState()
        {
            IsDefault = true;
            IsChoosingDirection = false;
            IsAddTag = false;
            IsAddName = false;
            IsAddStop = false;
            IsAddRoute = false;
            BufferDirection = null;
            BufferStopName = null;
            BufferRouteName = null;
            BufferTagName = null;
            BufferDirections = null;
        }

        [NotMapped]
        public Dictionary<string, string> BufferDirections
        {
            get { return JsonConvert.DeserializeObject<Dictionary<string, string>>(BufferDirection); }
            set { BufferDirection = JsonConvert.SerializeObject(value); }
        }
    }
}
