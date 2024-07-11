using ErrorOr;

namespace Gym.Domain;

public class Gym : Entity
{
    private readonly List<Guid> _gymRoomIds = [];
    private readonly List<Guid> _trainers = [];
    private readonly SubscriptionType _subscriptionType;

    public Gym(SubscriptionType subscriptionType, Guid? id = null) : base(id)
    {
        _subscriptionType = subscriptionType;
    }

    public static Gym CreateGym(SubscriptionType subscriptionType) 
    {
        return new Gym(subscriptionType);
    }

    public ErrorOr<Success> AddTrainer(Guid trainerId) 
    {
        _trainers.Add(trainerId);

        return Result.Success;
    }

    public ErrorOr<Success> RemoveTrainer(Guid trainerId)
    {
        _trainers.Remove(trainerId);

        return Result.Success;
    }

    public int GetMaxRooms() => _subscriptionType.MaxGymRoomCount;

    public ErrorOr<Success> AddGymRoom(GymRoom gymRoom)
    {
        if (_gymRoomIds.Count >= GetMaxRooms())
        {
            return GymRoomsErrors.CannotHaveMoreGymRoomsThanSubscritpionAllows;
        }

        _gymRoomIds.Add(gymRoom.Id);

        return Result.Success;
    }

    public ErrorOr<Success> RemoveGymRoom(GymRoom gymRoom)
    {
        if (!_gymRoomIds.Any())
        {
            return GymRoomsErrors.SubscriptionNotHaveGymRooms;
        }

        _gymRoomIds.Remove(gymRoom.Id);

        return Result.Success;
    }

    public int GetGymRoomCount()
    {
        return _gymRoomIds.Count;
    }
}
