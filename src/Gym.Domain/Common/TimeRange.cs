namespace Gym.Domain;

public record TimeRange
{
    public TimeOnly StartTime { get; init; }
    public TimeOnly EndTime { get; init; }

    private TimeRange(TimeOnly startTime, TimeOnly endTime) 
    {
        StartTime = startTime;
        EndTime = endTime;
    }

    public static TimeRange Create(TimeOnly startTime, TimeOnly endTime) 
    {
        if (startTime == default)
        {
            throw new ArgumentException("Недопустимое значение времени начала сессии");
        }

        if (endTime == default)
        {
            throw new ArgumentException("Недопустимое значение времени завершения сессии");
        }

        if (startTime > endTime)
        {
            throw new ArgumentException("Завершение не может быть раньше начала");
        }

        if (startTime == endTime)
        {
            throw new ArgumentException("Длительность сессии не может быть 0 минут");
        }

        return new TimeRange(startTime, endTime);
    }

    public bool GetSessionAlreadyEnd(DateOnly startDate, IDateTimeProvider dateTimeProvider) 
    {
        return (startDate.ToDateTime(EndTime) - dateTimeProvider.UtcNow)
            .TotalHours < 0;
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
