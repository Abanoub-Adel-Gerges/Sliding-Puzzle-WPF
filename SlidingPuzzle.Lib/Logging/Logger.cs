using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlidingPuzzle.Lib.Logging
{
    public class Logger : ILogger
    {
        private readonly string _filePath;
        private readonly StreamWriter _fileWriter;
        private readonly object _lock = new object();

        public Logger(string filePath = "log.txt")
        {
            _filePath = filePath;
            if (!File.Exists(_filePath))
            {
                using (File.Create(_filePath)) { }
            }
            _fileWriter = new StreamWriter(_filePath, append: true) { AutoFlush = true };
        }
        public void Log(object? obj, LoggerTypes type = LoggerTypes.Debug)
        {
            Log(obj, type, includeHeader: true);
        }
        public void Log(object? obj, LoggerTypes type = LoggerTypes.Debug, bool includeHeader = true)
        {
            string message;
            if (includeHeader)
            {
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                message = $"{timestamp} : [{type}] => {obj}";
            }
            else
            {
                message = obj?.ToString() ?? string.Empty;
            }

            lock (_lock)
            {
                _fileWriter.WriteLine(message);
            }
        }
        public void LogHeader(string title)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            lock (_lock)
            {
                _fileWriter.WriteLine($"\n{timestamp} --- STARTING TEST: {title} ---");
            }
        }
        public void Dispose()
        {
            _fileWriter?.Dispose();
        }
    }
}
