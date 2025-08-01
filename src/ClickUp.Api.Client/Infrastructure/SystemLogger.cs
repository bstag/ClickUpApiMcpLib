using System;
using Microsoft.Extensions.Logging;
using ClickUp.Api.Client.Abstractions.Infrastructure;
using MsLogLevel = Microsoft.Extensions.Logging.LogLevel;
using LogLevel = ClickUp.Api.Client.Abstractions.Infrastructure.LogLevel;

namespace ClickUp.Api.Client.Infrastructure;

/// <summary>
/// Concrete implementation of ILogger that wraps Microsoft.Extensions.Logging.ILogger.
/// This implementation provides dependency inversion by wrapping the .NET logging infrastructure,
/// supporting DI and testability while maintaining compatibility with the standard logging framework.
/// </summary>
/// <typeparam name="T">The type whose name is used for the logger category name.</typeparam>
public class SystemLogger<T> : ClickUp.Api.Client.Abstractions.Infrastructure.ILogger<T>
{
    private readonly Microsoft.Extensions.Logging.ILogger<T> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SystemLogger{T}"/> class.
    /// </summary>
    /// <param name="logger">The Microsoft.Extensions.Logging.ILogger instance to wrap.</param>
    /// <exception cref="ArgumentNullException">Thrown when logger is null.</exception>
    public SystemLogger(Microsoft.Extensions.Logging.ILogger<T> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public void Log(LogLevel logLevel, string message)
    {
        _logger.Log(ConvertLogLevel(logLevel), message);
    }

    /// <inheritdoc />
    public void Log(LogLevel logLevel, Exception? exception, string message)
    {
        _logger.Log(ConvertLogLevel(logLevel), exception, message);
    }

    /// <inheritdoc />
    public void Log(LogLevel logLevel, string message, params object[] args)
    {
        _logger.Log(ConvertLogLevel(logLevel), message, args);
    }

    /// <inheritdoc />
    public void Log(LogLevel logLevel, Exception? exception, string message, params object[] args)
    {
        _logger.Log(ConvertLogLevel(logLevel), exception, message, args);
    }

    /// <inheritdoc />
    public bool IsEnabled(LogLevel logLevel)
    {
        return _logger.IsEnabled(ConvertLogLevel(logLevel));
    }

    /// <inheritdoc />
    public void LogDebug(string message)
    {
        _logger.LogDebug(message);
    }

    /// <inheritdoc />
    public void LogDebug(string message, params object[] args)
    {
        _logger.LogDebug(message, args);
    }

    /// <inheritdoc />
    public void LogInformation(string message)
    {
        _logger.LogInformation(message);
    }

    /// <inheritdoc />
    public void LogInformation(string message, params object[] args)
    {
        _logger.LogInformation(message, args);
    }

    /// <inheritdoc />
    public void LogWarning(string message)
    {
        _logger.LogWarning(message);
    }

    /// <inheritdoc />
    public void LogWarning(Exception? exception, string message)
    {
        _logger.LogWarning(exception, message);
    }

    /// <inheritdoc />
    public void LogWarning(string message, params object[] args)
    {
        _logger.LogWarning(message, args);
    }

    /// <inheritdoc />
    public void LogWarning(Exception? exception, string message, params object[] args)
    {
        _logger.LogWarning(exception, message, args);
    }

    /// <inheritdoc />
    public void LogError(string message)
    {
        _logger.LogError(message);
    }

    /// <inheritdoc />
    public void LogError(Exception? exception, string message)
    {
        _logger.LogError(exception, message);
    }

    /// <inheritdoc />
    public void LogError(string message, params object[] args)
    {
        _logger.LogError(message, args);
    }

    /// <inheritdoc />
    public void LogError(Exception? exception, string message, params object[] args)
    {
        _logger.LogError(exception, message, args);
    }

    /// <inheritdoc />
    public void LogCritical(string message)
    {
        _logger.LogCritical(message);
    }

    /// <inheritdoc />
    public void LogCritical(Exception? exception, string message)
    {
        _logger.LogCritical(exception, message);
    }

    /// <inheritdoc />
    public void LogCritical(string message, params object[] args)
    {
        _logger.LogCritical(message, args);
    }

    /// <inheritdoc />
    public void LogCritical(Exception? exception, string message, params object[] args)
    {
        _logger.LogCritical(exception, message, args);
    }

    /// <summary>
    /// Converts the custom LogLevel to Microsoft.Extensions.Logging.LogLevel.
    /// </summary>
    /// <param name="logLevel">The custom log level to convert.</param>
    /// <returns>The corresponding Microsoft.Extensions.Logging.LogLevel.</returns>
    private static MsLogLevel ConvertLogLevel(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Trace => MsLogLevel.Trace,
            LogLevel.Debug => MsLogLevel.Debug,
            LogLevel.Information => MsLogLevel.Information,
            LogLevel.Warning => MsLogLevel.Warning,
            LogLevel.Error => MsLogLevel.Error,
            LogLevel.Critical => MsLogLevel.Critical,
            LogLevel.None => MsLogLevel.None,
            _ => MsLogLevel.Information
        };
    }
}