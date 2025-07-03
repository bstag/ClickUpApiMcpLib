using System;
using System.Collections.Generic;

namespace ClickUp.Api.Client.Models.Common.ValueObjects;

/// <summary>
/// Represents a range of time defined by a start and end date.
/// </summary>
/// <param name="StartDate">The start date of the time range. Its UTC representation will be used for conversion.</param>
/// <param name="EndDate">The end date of the time range. Its UTC representation will be used for conversion.</param>
public record TimeRange(DateTimeOffset StartDate, DateTimeOffset EndDate)
{
    // Compact constructor removed due to persistent build issues in the environment.
    // Validation is temporarily moved to ToQueryParameters.
    // public TimeRange
    // {
    //     if (StartDate > EndDate)
    //     {
    //         throw new ArgumentException("StartDate cannot be after EndDate.", nameof(StartDate));
    //     }
    // }

    /// <summary>
    /// Converts the TimeRange to a dictionary of query parameters.
    /// </summary>
    /// <param name="startDateParamName">The name for the start date parameter (defaults to "start_date").</param>
    /// <param name="endDateParamName">The name for the end date parameter (defaults to "end_date").</param>
    /// <returns>A dictionary containing the start and end dates as Unix time in milliseconds, based on their UTC representation.</returns>
    public Dictionary<string, string> ToQueryParameters(string startDateParamName = "start_date", string endDateParamName = "end_date")
    {
        // Temporary placement of validation due to build issues with compact constructor.
        if (this.StartDate > this.EndDate)
        {
            throw new ArgumentException("StartDate cannot be after EndDate.", nameof(StartDate));
        }
        return new Dictionary<string, string>
        {
            { startDateParamName, this.StartDate.ToUnixTimeMilliseconds().ToString() },
            { endDateParamName, this.EndDate.ToUnixTimeMilliseconds().ToString() }
        };
    }
}
