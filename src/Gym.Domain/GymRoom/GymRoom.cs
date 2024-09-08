using ErrorOr;

namespace Gym.Domain;

public class GymRoom : Entity
{
    private readonly Guid _gymId;
    private readonly List<Guid> _trainingSessionIds = [];
    private readonly SubscriptionType _subscriptionType;
    private readonly Shedule _shedule;
    private int _totalSpotCount;

    private int GetMaxDailySessions() => _subscriptionType.MaxDailySessionCount;

    public int AvailableSpotCount {
        get 
        {
            return _totalSpotCount - _trainingSessionIds.Count;
        }
    }

    public GymRoom(SubscriptionType subscriptionType, Shedule? shedule = null, Guid? id = null) : base(id)
    {
        _subscriptionType = subscriptionType;
        _shedule = shedule ?? new Shedule();
    }

    public ErrorOr<Success> ReserveTimeInShedule(DateOnly startDate, TimeRange timeRange)
    {
        return _shedule.AddTrainingSessionAtShedule(startDate, timeRange);
    }

    public ErrorOr<Success> UnreserveTimeInShedule(DateOnly startDate, TimeRange timeRange)
    {
        return _shedule.RemoveTrainingSessionFromShedule(startDate, timeRange);
    }

    public ErrorOr<Success> AddTrainingSession(Guid trainingSessionId)
    {
        if (_trainingSessionIds.Count >= GetMaxDailySessions())
        {
            return TrainingSessionErrors.CannotHaveMoreSessionsThanSubscritpionAllows;
        }

        _trainingSessionIds.Add(trainingSessionId);

        return Result.Success;
    }

    public ErrorOr<Success> RemoveTrainingSession(Guid trainingSessionId)
    {
        if (!_trainingSessionIds.Any())
        {
            return TrainingSessionErrors.SubscriptionNotHaveSessions;
        }

        if (!_trainingSessionIds.Contains(trainingSessionId))
        {
            return GymRoomsErrors.trainingSessionAlreadyNotExist;
        }

        _trainingSessionIds.Remove(trainingSessionId);

        return Result.Success;
    }

    public int GetDailySessionCount()
    {
        return _trainingSessionIds.Count;
    }
}
