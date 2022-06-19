using System;

namespace Carting.BL
{
    public interface ILogger
    {
        void LogException(Exception exception);
        void LogInfo(string message);
        void LogWarning(string message);
    }
}
