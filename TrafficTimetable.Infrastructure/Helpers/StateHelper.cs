
using System.Linq;
using TrafficTimetable.Domain;

namespace TrafficTimetable.Infrastructure.Helpers
{
    public static class StateHelper
    {

        public static Response GetStateInfo(ClientState clientState)
        {
            string stateInfo = null;
            switch (clientState.ClientStatus)
            {
                case Status.AddingName:
                    stateInfo = "хотели сообщить мне своё имя.";
                    break;
                case Status.AddingStop:
                    stateInfo = "хотели назвать мне название остановки.";
                    break;
                case Status.AddingTag:
                    stateInfo = $"хотели назвать тег для остановки {clientState.BufferStopName}, которую вы добавляли.";
                    break;
                case Status.AddingRoute:
                    stateInfo = $"хотели добавить маршрут к названной вами остановке {clientState.BufferStopName}.";
                    break;
                case Status.ChoosingDirection:
                    stateInfo = $"хотели выбрать направление маршрута {clientState.BufferRouteName} автобуса у остановки {clientState.BufferStopName}.";
                    break;
                case Status.AnnouncingTag:
                    return new Response("Куда направляемся?");
                default: return null;
            }
            return new Response($"Кажется, что наша работа была прервана. Вы {stateInfo} Хотите продолжить?",
                                new string[2] { "Да", "Нет" });
        }

        public static Response GetInstruction(ClientState clientState)
        {
            switch (clientState.ClientStatus)
            {
                case Status.AddingName:
                    return new Response("Как вас зовут?");
                case Status.AddingStop:
                    return new Response("Назовите название остановки");
                case Status.AddingTag:
                    return new Response("Назовите тег, который хотите привязать к этой остановке");
                case Status.AddingRoute:
                    return new Response("Назовите маршрут, время прибытия которого хотите узнать");
                case Status.ChoosingDirection:
                    return new Response($"Какое из направлений?\n 1. {clientState.BufferDirections.First().Key}\n " +
                        $"2.{clientState.BufferDirections.Last().Key}", new string[2] { "1", "2" });
                default:
                    return null;
            }
        }
    }
}
