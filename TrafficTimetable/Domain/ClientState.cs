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
            BufferTransportType = "автобуса";
        }

        [Key]
        public string ClientId { get; set; }

        public string SessionId { get; set; }

        public Status ClientStatus { get; set; }
        public bool WaitingToContinue { get; set; }
        public string BufferDirection { get; set; }
        public string BufferStopName { get; set; }
        public string BufferRouteName { get; set; }
        public string BufferTagName { get; set; }
        public string BufferTransportType { get; set; }

        public string GetStateInfo()
        {
            switch (ClientStatus)
            {
                case Status.AddingName:
                    return "хотели сообщить мне своё имя.";
                case Status.AddingStop:
                    return "хотели назвать мне название остановки.";
                case Status.AddingTag:
                    return $"хотели назвать тег для остановки {BufferStopName}, которую вы добавляли.";
                case Status.AddingRoute:
                    return $"хотели добавить маршрут к названной вами остановке {BufferStopName}.";
                case Status.ChoosingDirection:
                    return $"хотели выбрать направление маршрута {BufferRouteName} {BufferTransportType} у остановки {BufferStopName}.";
                default:
                    return null;
            }
        }

        public string GetInstruction()
        {
            switch (ClientStatus)
            {
                case Status.AddingName:
                    return "! Кажется, я вас вижу, ой, слышу впервые... давайте знакомиться! Как вас зовут?";
                case Status.AddingStop:
                    return "Назовите название остановки";
                case Status.AddingTag:
                    return "Назовите тег, который хотите привязать к этой остановке";
                case Status.AddingRoute:
                    return "Назовите маршрут, время прибытия которого хотите узнать";
                case Status.ChoosingDirection:
                    return $"Какое из направлений?\n 1. {BufferDirections.First().Key}\n 2.{BufferDirections.Last().Key}";
                default:
                    return null;
            }
        }

        [NotMapped]
        public Dictionary<string, string> BufferDirections
        {
            get { return JsonConvert.DeserializeObject<Dictionary<string, string>>(BufferDirection); }
            set { BufferDirection = JsonConvert.SerializeObject(value); }
        }
    }
}
