using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carting.Tests
{
    internal class FakeLogger : ILogger
    {
        public FakeLogger()
        {
            Events = new List<Event>();
        }

        public List<Event> Events { get; set; }

        public void LogException(Exception exception)
        {
            Events.Add(new Event(EventTypes.Error, exception.Message));
        }

        public void LogInfo(string message)
        {
            Events.Add(new Event(EventTypes.Info, message));
        }

        public void LogWarning(string message)
        {
            Events.Add(new Event(EventTypes.Warning, message));
        }
    }

    internal enum EventTypes
    {
        Info = 1,
        Warning = 2,
        Error = 3
    }

    internal class Event
    {
        public Event(EventTypes type, string message)
        {
            Type = type;
            Message = message;
            CreateDate = DateTime.Now;
        }

        public EventTypes Type { get; set; }
        public string Message { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
