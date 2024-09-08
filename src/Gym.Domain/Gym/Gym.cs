using ErrorOr;

namespace Gym.Domain;

public class Gym : Entity
{
    private readonly List<Guid> _gymRoomIds = [];
    private readonly List<Guid> _trainers = [];
    private readonly SubscriptionType _subscriptionType;

    private int GetMaxRoomCount() => _subscriptionType.MaxGymRoomCount;

    public Gym(SubscriptionType subscriptionType)
    {
        _subscriptionType = subscriptionType;
    }

    public static ErrorOr<Gym> CreateGym(SubscriptionType subscriptionType) 
    {
        if (subscriptionType is null) 
        {
            return GymErrors.SubscriptionIsNull;
        }

        return new Gym(subscriptionType);
    }

    public ErrorOr<Success> AddTrainer(Guid trainerId)
    {
        if (_trainers.Contains(trainerId)) 
        {
            return GymErrors.TrainerAlreadyExistInGym;
        }

        _trainers.Add(trainerId);

        return Result.Success;
    }

    public ErrorOr<Success> RemoveTrainer(Guid trainerId)
    {
        if (!_trainers.Contains(trainerId))
        {
            return GymErrors.TrainerAlreadyNotExistInGym;
        }

        _trainers.RemoveAll(trainer => trainer.Equals(trainerId));

        return Result.Success;
    }

    public ErrorOr<Success> AddGymRoom(Guid gymRoomId)
    {
        if (_gymRoomIds.Count >= GetMaxRoomCount())
        {
            return GymErrors.CannotHaveMoreGymRoomsThanSubscritpionAllows;
        }

        if (_gymRoomIds.Contains(gymRoomId))
        {
            return GymErrors.RoomAlreadyContainsInGym;
        }

        _gymRoomIds.Add(gymRoomId);

        return Result.Success;
    }

    public ErrorOr<Success> RemoveGymRoom(Guid roomId)
    {
        if (!_gymRoomIds.Any())
        {
            return GymErrors.SubscriptionNotHaveGymRooms;
        }

        if (!_gymRoomIds.Contains(roomId))
        {
            return GymErrors.RoomAlreadyNotExistInGym;
        }

        _gymRoomIds.RemoveAll(gymRoomId => gymRoomId.Equals(roomId));

        return Result.Success;
    }

    public int GetGymRoomCount()
    {
        return _gymRoomIds.Count;
    }
}
