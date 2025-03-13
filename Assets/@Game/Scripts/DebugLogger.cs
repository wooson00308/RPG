using UnityEngine;
using System;
using System.IO;
using System.Text;

public class DebugLogger : Singleton<DebugLogger>
{
    public enum LogLevel
    {
        None = 10,
        Info,
        Warning,
        Error
    }

    /// <summary>
    /// 콘솔에 출력할 로그의 최소 레벨입니다. 이 레벨 이상의 로그만 콘솔에 출력됩니다.
    /// </summary>
    [SerializeField] private LogLevel minimumConsoleLogLevel = LogLevel.Error;
    [SerializeField] private bool logToFile = false;  
    [SerializeField] private string logFileName = "DebugLog.txt"; 
    [SerializeField] private bool includeTimestamp = true; 

    private string logFilePath;

    public static void Log(LogLevel level, string message, object context = null)
    {
        Instance.LogMessage(level, message, context);
    }

    private void LogMessage(LogLevel level, string message, object context)
    {
        string formattedMessage = FormatMessage(level, message);

        bool isLoggable = (int)level >= (int)minimumConsoleLogLevel;
        if (isLoggable)
        {
            switch (level)
            {
                case LogLevel.Info:
                    Debug.Log(formattedMessage, context as UnityEngine.Object);
                    break;
                case LogLevel.Warning:
                    Debug.LogWarning(formattedMessage, context as UnityEngine.Object);
                    break;
                case LogLevel.Error:
                    Debug.LogError(formattedMessage, context as UnityEngine.Object);
                    break;
            }
        }

        if (logToFile)
        {
            WriteToFile(formattedMessage);
        }
    }

    private string FormatMessage(LogLevel level, string message)
    {
        StringBuilder formattedMessage = new StringBuilder();

        if (includeTimestamp)
        {
            formattedMessage.Append($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ");
        }

        formattedMessage.Append($"[{level}] ");
        formattedMessage.Append(message);

        return formattedMessage.ToString();
    }

    private void WriteToFile(string message)
    {
        try
        {
            string directory = Path.GetDirectoryName(logFilePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using (StreamWriter writer = new StreamWriter(logFilePath, true))
            {
                writer.WriteLine(message);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to write to log file: {e.Message}");
        }
    }

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(logFileName))
        {
            logFileName = "DebugLog.txt";
        }
        logFilePath = Path.Combine(Application.persistentDataPath, logFileName);
    }

    [ContextMenu("Test Logs")]
    private void TestLogs()
    {
        Log(LogLevel.Info, "This is an info message.");
        Log(LogLevel.Warning, "This is a warning message.");
        Log(LogLevel.Error, "This is an error message.");
    }
}

public static class Logger
{
    public static void Log(string message, object context = null)
    {
        DebugLogger.Log(DebugLogger.LogLevel.Info, message, context);
    }

    public static void LogWarning(string message, object context = null)
    {
        DebugLogger.Log(DebugLogger.LogLevel.Warning, message, context);
    }

    public static void LogError(string message, object context = null)
    {
        DebugLogger.Log(DebugLogger.LogLevel.Error, message, context);
    }
}