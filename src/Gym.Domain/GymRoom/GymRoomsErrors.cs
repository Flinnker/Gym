using ErrorOr;

namespace Gym.Domain;

public static class GymRoomsErrors
{
    public static readonly Error CannotHaveMoreGymRoomsThanSubscritpionAllows = Error.Validation(
        code: "Subscription_001", "CannotHaveMoreGymRoomsThanSubscritpionAllows");

    public static readonly Error SubscriptionNotHaveGymRooms = Error.Validation(
        code: "Subscription_002", "SubscriptionNotHaveGymRooms");
}
