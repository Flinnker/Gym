namespace Gym.Domain.Tests.Unit.Common;

public static class SessionInfoFactory
{
    public static SessionInfo CreateSessionInfo(
        TimeOnly startTime,
        TimeOnly endTime,
        Guid? sessionId = null,
        DateOnly? startDate = null)
    {
        var timeRange = new Domain.TimeRange(
            startTime: startTime,
            endTime: endTime);

        return new SessionInfo()
        {
            SessionId = sessionId ?? Guid.NewGuid(),
            StartDate = startDate ?? new DateOnly(),
            TimeRange = timeRange
        };
    }
}
