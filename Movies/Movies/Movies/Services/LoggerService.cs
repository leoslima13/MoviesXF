using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Movies.Services
{
    public interface ILoggerService
    {
        void Info(string message, [CallerMemberName] string caller = null);
        void Error(string errorMessage, Dictionary<string, string> props = null, [CallerMemberName] string caller = null);
        void Error(Exception ex, Dictionary<string, string> props = null, [CallerMemberName] string caller = null);
        void Error(string errorMessage, Exception ex, Dictionary<string, string> props = null, [CallerMemberName] string caller = null);
        void Log(string eventName, string message = null, [CallerMemberName] string caller = null);
    }
    
    public class LoggerService : ILoggerService
    {
        const string TAG = "[App]";

        public void Info(string message, [CallerMemberName] string caller = null) =>
            Console.WriteLine($"[{TAG}] [{caller}] [DEBUG] - {message}");

        public void Error(string errorMessage, Dictionary<string, string> props = null, [CallerMemberName] string caller = null) =>
            Console.WriteLine($"[{TAG}] [{caller}] [ERROR] - {errorMessage}");

        public void Error(string errorMessage, Exception ex, Dictionary<string, string> props = null, [CallerMemberName] string caller = null) =>
            Console.WriteLine($"[{TAG}] [{caller}] [ERROR] - {errorMessage}\n{ex.GetType().Name}: {ex}");

        public void Error(Exception ex, Dictionary<string, string> props = null, [CallerMemberName] string caller = null) =>
            Console.WriteLine($"[{TAG}] [{caller}] [ERROR] - {ex.GetType().Name}: {ex}");

        public void Log(string eventName, string message = null, [CallerMemberName] string caller = null)
        {
            Console.WriteLine($"[{TAG}] [{caller}] [INFO] - {eventName}: {message}");
        }
    }
}