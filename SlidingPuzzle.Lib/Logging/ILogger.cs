using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlidingPuzzle.Lib.Logging
{
    public interface ILogger : IDisposable
    {
        void Log(object? obj, LoggerTypes type = LoggerTypes.Debug);
        void Log(object? obj, LoggerTypes type, bool includeHeader);
        void LogHeader(string title);
    }
}
