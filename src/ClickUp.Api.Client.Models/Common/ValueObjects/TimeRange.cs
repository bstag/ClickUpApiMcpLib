using System;
using System.Collections.Generic;

namespace ClickUp.Api.Client.Models.Common.ValueObjects;

/// <summary>
/// Represents a range of time defined by a start and end date.
/// </summary>
public record TimeRange
{
    public DateTimeOffset StartDate { get; }
    public DateTimeOffset EndDate { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TimeRange"/> record.
    /// </summary>
    /// <param name="startDate">The start date of the time range. Its UTC representation will be used for conversion.</param>
    /// <param name="endDate">The end date of the time range. Its UTC representation will be used for conversion.</param>
    /// <exception cref="ArgumentException">Thrown if startDate is after endDate.</exception>
    public TimeRange(DateTimeOffset startDate, DateTimeOffset endDate)
    {
        if (startDate > endDate)
        {
            throw new ArgumentException("StartDate cannot be after EndDate.", nameof(startDate));
        }
        StartDate = startDate;
        EndDate = endDate;
    }

    /// <summary>
    /// Converts the TimeRange to a dictionary of query parameters.
    /// </summary>
    /// <param name="startDateParamName">The name for the start date parameter (defaults to "start_date").</param>
    /// <param name="endDateParamName">The name for the end date parameter (defaults to "end_date").</param>
    /// <returns>A dictionary containing the start and end dates as Unix time in milliseconds, based on their UTC representation.</returns>
    public Dictionary<string, string> ToQueryParameters(string startDateParamName = "start_date", string endDateParamName = "end_date")
    {
        return new Dictionary<string, string>
        {
            { startDateParamName, StartDate.ToUnixTimeMilliseconds().ToString() },
            { endDateParamName, EndDate.ToUnixTimeMilliseconds().ToString() }
        };
    }
}
