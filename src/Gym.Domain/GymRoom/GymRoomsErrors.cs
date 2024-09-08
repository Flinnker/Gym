using ErrorOr;

namespace Gym.Domain;

public static class GymRoomsErrors
{
    public static readonly Error CannotHaveMoreGymRoomsThanSubscritpionAllows = Error.Validation(
        code: "Subscription_001", "CannotHaveMoreGymRoomsThanSubscritpionAllows");

    public static readonly Error SubscriptionNotHaveGymRooms = Error.Validation(
        code: "Subscription_002", "SubscriptionNotHaveGymRooms");

    public static readonly Error TimeAlreadyTaken = Error.Validation(
        code: "Subscription_003", "TimeAlreadyTaken");

    public static readonly Error TimeIsNotTaken = Error.Validation(
        code: "Subscription_004", "TimeIsNotTaken");

    public static readonly Error trainingSessionAlreadyNotExist = Error.Validation(
        code: "Subscription_005", "trainingSessionAlreadyNotExist");
}
