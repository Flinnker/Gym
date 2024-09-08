using ErrorOr;

namespace Gym.Domain;

public record TimeRange
{
    public TimeOnly StartTime { get; init; }
    public TimeOnly EndTime { get; init; }

    public TimeRange(TimeOnly startTime, TimeOnly endTime) 
    {
        StartTime = startTime;
        EndTime = endTime;
    }

    public static ErrorOr<TimeRange> Create(TimeOnly startTime, TimeOnly endTime) 
    {
        if (startTime == default)
        {
            return TimeRangeErrors.NotAvailableSessionBeginTime;
        }

        if (endTime == default)
        {
            return TimeRangeErrors.NotAvailableSessionEndTime;
        }

        if (startTime > endTime)
        {
            return TimeRangeErrors.EndCanNotBeEarlierThenBegin;
        }

        if (startTime == endTime)
        {
            return TimeRangeErrors.DurationCanNotBeZeroMinutes;
        }

        return new TimeRange(startTime, endTime);
    }

    public bool GetSessionAlreadyEnd(DateOnly startDate, IDateTimeProvider dateTimeProvider) 
    {
        return (startDate.ToDateTime(EndTime) - dateTimeProvider.UtcNow)
            .TotalHours <= 0;
    }

    public bool GetUserLateForCancel(DateOnly startDate, IDateTimeProvider dateTimeProvider)
    {
        return (startDate.ToDateTime(StartTime) - dateTimeProvider.UtcNow)
            .TotalHours < Constants.TrainingSession.HourseToCancelSession;
    }

    public bool GetIsTimeRangeOverlapping(TimeRange timeRange) 
    {
        return !(
            StartTime >= timeRange.EndTime ||
            EndTime <= timeRange.StartTime);
    }
}
