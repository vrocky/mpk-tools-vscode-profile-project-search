using System.IO;
using System.Text;

namespace VsCodeProfileProjectSearch.Services;

public static class LoggingService
{
    private static readonly object Sync = new();

    public static string LogDirectory
    {
        get
        {
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            if (string.IsNullOrWhiteSpace(localAppData))
            {
                localAppData = AppContext.BaseDirectory;
            }

            var dir = Path.Combine(
                localAppData,
                "VsCodeProfileProjectSearch",
                "logs");
            Directory.CreateDirectory(dir);
            return dir;
        }
    }

    public static string CurrentLogFile => Path.Combine(LogDirectory, $"app-{DateTime.Now:yyyyMMdd}.log");

    public static void Info(string message)
    {
        Write("INFO", message, null);
    }

    public static void Error(string message, Exception? exception = null)
    {
        Write("ERROR", message, exception);
    }

    private static void Write(string level, string message, Exception? exception)
    {
        try
        {
            var builder = new StringBuilder();
            builder.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            builder.Append(" [").Append(level).Append("] ").AppendLine(message);

            if (exception is not null)
            {
                builder.AppendLine(exception.ToString());
            }

            lock (Sync)
            {
                File.AppendAllText(CurrentLogFile, builder.ToString());
            }
        }
        catch
        {
            // Avoid recursive failures if logging itself breaks.
        }
    }
}
