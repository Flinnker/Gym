using ErrorOr;

namespace Gym.Domain;

public class Shedule : Entity
{
    private Dictionary<DateOnly, List<TimeRange>> _trainingSessionShedule = [];

    private Shedule()
    { }

    public Shedule(Guid? id = null) : base(id)
    { }

    public static Shedule CreateShedule() 
    {
        return new Shedule(Guid.NewGuid());
    }

    public ErrorOr<Success> AddTrainingSessionAtShedule(DateOnly startDate, TimeRange addingTimeRange) 
    {
        if (!_trainingSessionShedule.TryGetValue(startDate, out var timeRanges))
        {
            _trainingSessionShedule.Add(startDate, new List<TimeRange>() 
            {
                addingTimeRange
            });

            return Result.Success;
        }

        foreach (var timeRange in timeRanges)
        {
            if (timeRange.GetIsTimeRangeOverlapping(addingTimeRange))
            {
                return SheduleError.OverlappReservedTrainingSession;
            }
        }

        timeRanges.Add(addingTimeRange);

        return Result.Success;
    }

    public ErrorOr<Success> RemoveTrainingSessionFromShedule(DateOnly startDate, TimeRange addingTimeRange) 
    {
        if (!_trainingSessionShedule.TryGetValue(startDate, out var timeRanges))
        {
            return SheduleError.TrainingSessionNotFound;
        }

        var timeRangeIndex = timeRanges.FindIndex(timeRange => 
            timeRange.StartTime == addingTimeRange.StartTime && 
            timeRange.EndTime == addingTimeRange.EndTime);

        if (timeRangeIndex == -1) 
        {
            return SheduleError.TrainingSessionNotFound;
        }

        timeRanges.RemoveAt(timeRangeIndex);

        return Result.Success;
    }

    public bool GetTimeRangeIsFree(DateOnly startDate, TimeRange checkingTimeRange) 
    {
        if (!_trainingSessionShedule.TryGetValue(startDate, out var timeRanges))
        {
            return true;
        }

        foreach (var timeRange in timeRanges)
        {
            if (timeRange.GetIsTimeRangeOverlapping(checkingTimeRange))
            {
                return false;
            }
        }

        return true;
    }
}
