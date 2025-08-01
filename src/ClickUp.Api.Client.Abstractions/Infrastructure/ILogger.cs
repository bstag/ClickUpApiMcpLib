using System;

namespace ClickUp.Api.Client.Abstractions.Infrastructure;

/// <summary>
/// Abstraction for logging operations to support dependency inversion principle.
/// This interface provides a testable abstraction over logging functionality,
/// allowing for easy mocking and testing of components that require logging.
/// </summary>
/// <typeparam name="T">The type whose name is used for the logger category name.</typeparam>
public interface ILogger<T>
{
    /// <summary>
    /// Writes a log entry with the specified log level.
    /// </summary>
    /// <param name="logLevel">The log level.</param>
    /// <param name="message">The log message.</param>
    void Log(LogLevel logLevel, string message);

    /// <summary>
    /// Writes a log entry with the specified log level and exception.
    /// </summary>
    /// <param name="logLevel">The log level.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">The log message.</param>
    void Log(LogLevel logLevel, Exception? exception, string message);

    /// <summary>
    /// Writes a log entry with the specified log level, message template, and arguments.
    /// </summary>
    /// <param name="logLevel">The log level.</param>
    /// <param name="message">The message template.</param>
    /// <param name="args">The arguments to format into the message template.</param>
    void Log(LogLevel logLevel, string message, params object[] args);

    /// <summary>
    /// Writes a log entry with the specified log level, exception, message template, and arguments.
    /// </summary>
    /// <param name="logLevel">The log level.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">The message template.</param>
    /// <param name="args">The arguments to format into the message template.</param>
    void Log(LogLevel logLevel, Exception? exception, string message, params object[] args);

    /// <summary>
    /// Checks if the given log level is enabled.
    /// </summary>
    /// <param name="logLevel">The log level to check.</param>
    /// <returns>True if the log level is enabled; otherwise, false.</returns>
    bool IsEnabled(LogLevel logLevel);

    /// <summary>
    /// Writes a debug log entry.
    /// </summary>
    /// <param name="message">The log message.</param>
    void LogDebug(string message);

    /// <summary>
    /// Writes a debug log entry with arguments.
    /// </summary>
    /// <param name="message">The message template.</param>
    /// <param name="args">The arguments to format into the message template.</param>
    void LogDebug(string message, params object[] args);

    /// <summary>
    /// Writes an information log entry.
    /// </summary>
    /// <param name="message">The log message.</param>
    void LogInformation(string message);

    /// <summary>
    /// Writes an information log entry with arguments.
    /// </summary>
    /// <param name="message">The message template.</param>
    /// <param name="args">The arguments to format into the message template.</param>
    void LogInformation(string message, params object[] args);

    /// <summary>
    /// Writes a warning log entry.
    /// </summary>
    /// <param name="message">The log message.</param>
    void LogWarning(string message);

    /// <summary>
    /// Writes a warning log entry with exception.
    /// </summary>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">The log message.</param>
    void LogWarning(Exception? exception, string message);

    /// <summary>
    /// Writes a warning log entry with arguments.
    /// </summary>
    /// <param name="message">The message template.</param>
    /// <param name="args">The arguments to format into the message template.</param>
    void LogWarning(string message, params object[] args);

    /// <summary>
    /// Writes a warning log entry with exception and arguments.
    /// </summary>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">The message template.</param>
    /// <param name="args">The arguments to format into the message template.</param>
    void LogWarning(Exception? exception, string message, params object[] args);

    /// <summary>
    /// Writes an error log entry.
    /// </summary>
    /// <param name="message">The log message.</param>
    void LogError(string message);

    /// <summary>
    /// Writes an error log entry with exception.
    /// </summary>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">The log message.</param>
    void LogError(Exception? exception, string message);

    /// <summary>
    /// Writes an error log entry with arguments.
    /// </summary>
    /// <param name="message">The message template.</param>
    /// <param name="args">The arguments to format into the message template.</param>
    void LogError(string message, params object[] args);

    /// <summary>
    /// Writes an error log entry with exception and arguments.
    /// </summary>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">The message template.</param>
    /// <param name="args">The arguments to format into the message template.</param>
    void LogError(Exception? exception, string message, params object[] args);

    /// <summary>
    /// Writes a critical log entry.
    /// </summary>
    /// <param name="message">The log message.</param>
    void LogCritical(string message);

    /// <summary>
    /// Writes a critical log entry with exception.
    /// </summary>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">The log message.</param>
    void LogCritical(Exception? exception, string message);

    /// <summary>
    /// Writes a critical log entry with arguments.
    /// </summary>
    /// <param name="message">The message template.</param>
    /// <param name="args">The arguments to format into the message template.</param>
    void LogCritical(string message, params object[] args);

    /// <summary>
    /// Writes a critical log entry with exception and arguments.
    /// </summary>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">The message template.</param>
    /// <param name="args">The arguments to format into the message template.</param>
    void LogCritical(Exception? exception, string message, params object[] args);
}

/// <summary>
/// Defines logging severity levels.
/// </summary>
public enum LogLevel
{
    /// <summary>
    /// Logs that contain the most detailed messages. These messages may contain sensitive application data.
    /// These messages are disabled by default and should never be enabled in a production environment.
    /// </summary>
    Trace = 0,

    /// <summary>
    /// Logs that are used for interactive investigation during development. These logs should primarily contain
    /// information useful for debugging and have no long-term value.
    /// </summary>
    Debug = 1,

    /// <summary>
    /// Logs that track the general flow of the application. These logs should have long-term value.
    /// </summary>
    Information = 2,

    /// <summary>
    /// Logs that highlight an abnormal or unexpected event in the application flow, but do not otherwise cause the
    /// application execution to stop.
    /// </summary>
    Warning = 3,

    /// <summary>
    /// Logs that highlight when the current flow of execution is stopped due to a failure. These should indicate a
    /// failure in the current activity, not an application-wide failure.
    /// </summary>
    Error = 4,

    /// <summary>
    /// Logs that describe an unrecoverable application or system crash, or a catastrophic failure that requires
    /// immediate attention.
    /// </summary>
    Critical = 5,

    /// <summary>
    /// Not used for writing log messages. Specifies that a logging category should not write any messages.
    /// </summary>
    None = 6
}