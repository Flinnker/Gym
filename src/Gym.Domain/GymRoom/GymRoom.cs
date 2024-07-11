using ErrorOr;

namespace Gym.Domain;

public class GymRoom : Entity
{
    private readonly Guid _gymId;
    private readonly List<Guid> _trainingSessionIds = [];
    private readonly SubscriptionType _subscriptionType;
    private readonly Shedule _shedule;
    private int _totalSpotCount;

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

    public ErrorOr<Success> ReserveTrainingSession(DateOnly startDate, TimeRange timeRange)
    {
        if (_shedule.AddTrainingSessionAtShedule(startDate, timeRange) != Result.Success)
        {
            return ParticipantErrors.TrainingSessionOverlappingReservedSession;
        }

        return Result.Success;
    }

    public ErrorOr<Success> CancelTrainingSession(DateOnly startDate, TimeRange timeRange)
    {
        var removeResult = _shedule.RemoveTrainingSessionFromShedule(startDate, timeRange);

        if (removeResult != Result.Success)
        {
            return removeResult;
        }

        return Result.Success;
    }

    public int GetMaxDailySessions() => _subscriptionType.MaxDailySessionCount;

    public ErrorOr<Success> AddTrainingSession(TrainingSession trainingSession)
    {
        if (_trainingSessionIds.Count >= GetMaxDailySessions())
        {
            return TrainingSessionErrors.CannotHaveMoreSessionsThanSubscritpionAllows;
        }

        _trainingSessionIds.Add(trainingSession.Id);

        return Result.Success;
    }

    public ErrorOr<Success> RemoveTrainingSession(TrainingSession trainingSession)
    {
        if (!_trainingSessionIds.Any())
        {
            return TrainingSessionErrors.SubscriptionNotHaveSessions;
        }

        _trainingSessionIds.Remove(trainingSession.Id);

        return Result.Success;
    }

    public int GetDailySessionCount()
    {
        return _trainingSessionIds.Count;
    }
}
