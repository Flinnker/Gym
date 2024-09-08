using ErrorOr;

namespace Gym.Domain;

public class Subscription : Entity
{
    private readonly SubscriptionType _type;
    private readonly List<Guid> _gymIds;
    private readonly List<Guid> _gymRoomIds;
    private readonly List<Guid> _trainingSessionIds;

    public bool Active { get; private set; } = true;

    public IReadOnlyCollection<Guid> GymIds 
    {
        get => _gymIds.AsReadOnly();
    }
    public IReadOnlyCollection<Guid> GymRoomIds
    {
        get => _gymRoomIds.AsReadOnly();
    }
    public IReadOnlyCollection<Guid> TrainingSessionIds
    {
        get => _trainingSessionIds.AsReadOnly();
    }

    public Subscription(
        SubscriptionType type,
        Guid? id = null,
        List<Guid>? gymIds = null,
        List<Guid>? gymRoomIds = null,
        List<Guid>? trainingSessionIds = null) : base(id)
    {
        _type = type;
        _gymIds = gymIds ?? [];
        _gymRoomIds = gymRoomIds ?? [];
        _trainingSessionIds = trainingSessionIds ?? [];
    }

    public int GetMaxGyms() => _type.MaxGymCount;

    public int GetMaxRooms() => _type.MaxGymRoomCount;

    public int GetMaxDailySessions() => _type.MaxDailySessionCount;

    public int GetPrice() => _type.Price;


    public ErrorOr<Success> AddGym(Guid gymId)
    {
        if (_gymIds.Count >= GetMaxGyms()) 
        {
            return GymErrors.CannotHaveMoreGymsThanSubscritpionAllows;
        }

        _gymIds.Add(gymId);

        return Result.Success;
    }

    public ErrorOr<Success> RemoveGym(Guid gymId)
    {
        if (!_gymIds.Any())
        {
            return GymErrors.SubscriptionNotHaveGyms;
        }

        if (!_gymIds.Contains(gymId))
        {
            return SubscriptionErrors.SubscriptionNotHaveThisGym;
        }

        _gymIds.RemoveAll(localGymId => localGymId.Equals(gymId));

        return Result.Success;
    }

    public int GetGymCount()
    {
        return _gymIds.Count;
    }
}

