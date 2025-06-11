using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// FileLogger.cs

namespace PaymentGatewayIntegration.Logging
{
    public class FileLogger
    {
        private readonly string _logFilePath;
        private static readonly object _lock = new object();

        public FileLogger(string logFilePath)
        {
            _logFilePath = logFilePath;

            // Garantir que o diretório existe
            var directory = Path.GetDirectoryName(_logFilePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        public void LogInfo(string message)
        {
            Log("INFO", message);
        }

        public void LogError(string message, Exception ex = null)
        {
            var errorMessage = ex != null
                ? $"{message}. Exception: {ex.Message}. StackTrace: {ex.StackTrace}"
                : message;

            Log("ERROR", errorMessage);
        }

        public void LogWarning(string message)
        {
            Log("WARNING", message);
        }

        private void Log(string level, string message)
        {
            var logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}] {message}{Environment.NewLine}";

            lock (_lock)
            {
                File.AppendAllText(_logFilePath, logEntry, Encoding.UTF8);
            }
        }
    }
}