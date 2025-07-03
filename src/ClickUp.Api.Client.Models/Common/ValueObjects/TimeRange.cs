using System;
using System.Collections.Generic;

namespace ClickUp.Api.Client.Models.Common.ValueObjects
{
    public record TimeRange
    {
        public DateTimeOffset StartDate { get; }
        public DateTimeOffset EndDate { get; }

        public TimeRange(DateTimeOffset startDate, DateTimeOffset endDate)
        {
            if (startDate > endDate)
            {
                throw new ArgumentException($"{nameof(startDate)} cannot be later than {nameof(endDate)}.");
            }
            StartDate = startDate;
            EndDate = endDate;
        }

        public Dictionary<string, string> ToQueryParameters(string startDateParamName = "start_date", string endDateParamName = "end_date")
        {
            return new Dictionary<string, string>
            {
                { startDateParamName, new DateTimeOffset(StartDate.UtcDateTime).ToUnixTimeMilliseconds().ToString() },
                { endDateParamName, new DateTimeOffset(EndDate.UtcDateTime).ToUnixTimeMilliseconds().ToString() }
            };
        }
    }
}
