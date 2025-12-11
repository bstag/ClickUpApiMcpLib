using System;

namespace ClickUp.Api.Client.Abstractions.Infrastructure
{
    /// <summary>
    /// Abstraction for date and time operations to support dependency inversion principle and testability.
    /// This interface provides a testable abstraction over system date/time operations.
    /// </summary>
    /// <remarks>
    /// This abstraction allows for:
    /// - Easy unit testing by providing mock implementations with fixed dates/times
    /// - Time zone handling and conversion
    /// - Consistent date/time operations across the application
    /// - Support for different date/time strategies (UTC, local, specific time zones)
    /// </remarks>
    public interface IDateTimeProvider
    {
        /// <summary>
        /// Gets the current date and time in UTC.
        /// </summary>
        /// <returns>The current UTC date and time.</returns>
        DateTime UtcNow { get; }

        /// <summary>
        /// Gets the current local date and time.
        /// </summary>
        /// <returns>The current local date and time.</returns>
        DateTime Now { get; }

        /// <summary>
        /// Gets the current date (without time component) in UTC.
        /// </summary>
        /// <returns>The current UTC date.</returns>
        DateTime UtcToday { get; }

        /// <summary>
        /// Gets the current local date (without time component).
        /// </summary>
        /// <returns>The current local date.</returns>
        DateTime Today { get; }

        /// <summary>
        /// Converts a UTC DateTime to the local time zone.
        /// </summary>
        /// <param name="utcDateTime">The UTC DateTime to convert.</param>
        /// <returns>The DateTime converted to local time.</returns>
        DateTime ToLocalTime(DateTime utcDateTime);

        /// <summary>
        /// Converts a local DateTime to UTC.
        /// </summary>
        /// <param name="localDateTime">The local DateTime to convert.</param>
        /// <returns>The DateTime converted to UTC.</returns>
        DateTime ToUniversalTime(DateTime localDateTime);

        /// <summary>
        /// Gets the current Unix timestamp (seconds since epoch).
        /// </summary>
        /// <returns>The current Unix timestamp.</returns>
        long UnixTimestamp { get; }

        /// <summary>
        /// Converts a DateTime to Unix timestamp.
        /// </summary>
        /// <param name="dateTime">The DateTime to convert.</param>
        /// <returns>The Unix timestamp representation.</returns>
        long ToUnixTimestamp(DateTime dateTime);

        /// <summary>
        /// Converts a Unix timestamp to DateTime.
        /// </summary>
        /// <param name="unixTimestamp">The Unix timestamp to convert.</param>
        /// <returns>The DateTime representation of the Unix timestamp.</returns>
        DateTime FromUnixTimestamp(long unixTimestamp);
    }
}