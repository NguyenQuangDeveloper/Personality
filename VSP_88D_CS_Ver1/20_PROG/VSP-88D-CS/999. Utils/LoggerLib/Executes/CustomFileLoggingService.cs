using LoggerLib.Interfaces;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace LoggerLib.Executes;

public class CustomFileLoggingService : ILoggingService
{
    private static readonly object _lock = new();

    public string Path { get; set; }
    public string Prefix { get; set; }

    public CustomFileLoggingService()
    {

    }

    private void WriteLog(string content, string level)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(Path))
            {
                Debug.WriteLine("[CustomLogger] Log path is null or empty.");
                return;
            }

            var fileName = $"{DateTime.Now:yyyy-MM-dd}.log";
            if (!Directory.Exists(Path))
                Directory.CreateDirectory(Path);

            var filePath = System.IO.Path.Combine(Path, fileName);
            var log = $"\r\n{DateTime.Now:HH:mm:ss.ff} | {level} | {Prefix} | {content}";

            lock (_lock)
            {
                try
                {
                    Debug.Write(log);
                    using (var writer = new StreamWriter(filePath, append: true, encoding: System.Text.Encoding.UTF8))
                    {
                        writer.WriteLine(log);
                        writer.Flush();
                    }
                }
                catch (IOException ioEx)
                {
                    Debug.WriteLine($"[CustomLogger][IO] {ioEx.Message}");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[CustomLogger][General] {ex.Message}");
                }
            }
        }
        catch (Exception fatalEx)
        {
            Debug.WriteLine($"[CustomLogger][FATAL] {fatalEx.Message}");
        }
    }


    public void LogInfo(
        string message,
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0,
        [CallerMemberName] string member = ""
    ) => WriteLog(message, "INFO");

    public void LogWarning(
        string message,
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0,
        [CallerMemberName] string member = ""
    ) => WriteLog(message, "WARN");

    public void LogError(
        string message,
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0,
        [CallerMemberName] string member = "",
        Exception? ex = null
    ) => WriteLog($"{message} | {ex}", "ERROR");

    public void LogDebug(
        string message,
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0,
        [CallerMemberName] string member = ""
    ) => WriteLog(message, "DEBUG");
}
