namespace Gym.Domain.Tests.Unit.TrainingSession;

public static class TrainingSessionFactory
{
    public static Domain.TrainingSession CreateTrainingSession(
        int sessionSize,
        TimeOnly startTime,
        TimeOnly endTime,
        DateOnly startSessionDate)
    {
        var gymRoomId = Guid.NewGuid();
        var trainerId = Guid.NewGuid();
        var timeRange = new Domain.TimeRange(
            startTime: startTime,
            endTime: endTime);
        var trainingSession = new Domain.TrainingSession(
            gymRoomId: gymRoomId,
            sessionSize: sessionSize,
            trainerId: trainerId,
            startSessionDate: startSessionDate,
            startSessionTime: timeRange.StartTime,
            endSessionTime: timeRange.EndTime);

        return trainingSession;
    }
}
