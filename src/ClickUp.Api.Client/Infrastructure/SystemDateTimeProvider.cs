using System;
using ClickUp.Api.Client.Abstractions.Infrastructure;

namespace ClickUp.Api.Client.Infrastructure
{
    /// <summary>
    /// Concrete implementation of IDateTimeProvider that uses system date/time operations.
    /// This implementation provides dependency inversion while using standard system DateTime operations.
    /// </summary>
    /// <remarks>
    /// This implementation allows the ClickUp SDK to:
    /// - Use standard system date/time operations for production scenarios
    /// - Support dependency injection and testability
    /// - Provide consistent date/time handling across the application
    /// - Support Unix timestamp conversions for API compatibility
    /// </remarks>
    public class SystemDateTimeProvider : IDateTimeProvider
    {
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Gets the current date and time in UTC.
        /// </summary>
        /// <returns>The current UTC date and time.</returns>
        public DateTime UtcNow => DateTime.UtcNow;

        /// <summary>
        /// Gets the current local date and time.
        /// </summary>
        /// <returns>The current local date and time.</returns>
        public DateTime Now => DateTime.Now;

        /// <summary>
        /// Gets the current date (without time component) in UTC.
        /// </summary>
        /// <returns>The current UTC date.</returns>
        public DateTime UtcToday => DateTime.UtcNow.Date;

        /// <summary>
        /// Gets the current local date (without time component).
        /// </summary>
        /// <returns>The current local date.</returns>
        public DateTime Today => DateTime.Today;

        /// <summary>
        /// Gets the current Unix timestamp (seconds since epoch).
        /// </summary>
        /// <returns>The current Unix timestamp.</returns>
        public long UnixTimestamp => ToUnixTimestamp(UtcNow);

        /// <summary>
        /// Converts a UTC DateTime to the local time zone.
        /// </summary>
        /// <param name="utcDateTime">The UTC DateTime to convert.</param>
        /// <returns>The DateTime converted to local time.</returns>
        /// <exception cref="ArgumentException">Thrown when the input DateTime is not UTC.</exception>
        public DateTime ToLocalTime(DateTime utcDateTime)
        {
            if (utcDateTime.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException("Input DateTime must be UTC.", nameof(utcDateTime));
            }

            return utcDateTime.ToLocalTime();
        }

        /// <summary>
        /// Converts a local DateTime to UTC.
        /// </summary>
        /// <param name="localDateTime">The local DateTime to convert.</param>
        /// <returns>The DateTime converted to UTC.</returns>
        /// <exception cref="ArgumentException">Thrown when the input DateTime is not local or unspecified.</exception>
        public DateTime ToUniversalTime(DateTime localDateTime)
        {
            if (localDateTime.Kind == DateTimeKind.Utc)
            {
                throw new ArgumentException("Input DateTime is already UTC.", nameof(localDateTime));
            }

            return localDateTime.ToUniversalTime();
        }

        /// <summary>
        /// Converts a DateTime to Unix timestamp.
        /// </summary>
        /// <param name="dateTime">The DateTime to convert.</param>
        /// <returns>The Unix timestamp representation.</returns>
        /// <remarks>
        /// If the input DateTime is not UTC, it will be converted to UTC first.
        /// </remarks>
        public long ToUnixTimestamp(DateTime dateTime)
        {
            var utcDateTime = dateTime.Kind == DateTimeKind.Utc ? dateTime : dateTime.ToUniversalTime();
            return (long)(utcDateTime - UnixEpoch).TotalSeconds;
        }

        /// <summary>
        /// Converts a Unix timestamp to DateTime.
        /// </summary>
        /// <param name="unixTimestamp">The Unix timestamp to convert.</param>
        /// <returns>The DateTime representation of the Unix timestamp in UTC.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the Unix timestamp is negative.</exception>
        public DateTime FromUnixTimestamp(long unixTimestamp)
        {
            if (unixTimestamp < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(unixTimestamp), "Unix timestamp cannot be negative.");
            }

            return UnixEpoch.AddSeconds(unixTimestamp);
        }
    }
}