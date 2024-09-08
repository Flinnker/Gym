using ErrorOr;

namespace Gym.Domain;

public class GymErrors
{
    public static readonly Error CannotHaveMoreGymsThanSubscritpionAllows = Error.Validation(
        code: "Subscription_001", "CannotHaveMoreGymsThanSubscritpionAllows");

    public static readonly Error SubscriptionNotHaveGyms = Error.Validation(
        code: "Subscription_002", "SubscriptionNotHaveGyms");

    public static readonly Error SubscriptionIsNull = Error.Validation(
        code: "Subscription_003", "SubscriptionIsNull");

    public static readonly Error TrainerAlreadyExistInGym = Error.Validation(
        code: "Subscription_004", "TrainerAlreadyExistInGym");

    public static readonly Error TrainerAlreadyNotExistInGym = Error.Validation(
        code: "Subscription_005", "TrainerAlreadyNotExistInGym");

    public static readonly Error RoomAlreadyContainsInGym = Error.Validation(
        code: "Subscription_006", "RoomAlreadyContainsInGym");

    public static readonly Error CannotHaveMoreGymRoomsThanSubscritpionAllows = Error.Validation(
        code: "Subscription_007", "CannotHaveMoreGymRoomsThanSubscritpionAllows");

    public static readonly Error RoomAlreadyNotExistInGym = Error.Validation(
        code: "Subscription_008", "RoomAlreadyNotExistInGym");

    public static readonly Error SubscriptionNotHaveGymRooms = Error.Validation(
        code: "Subscription_009", "SubscriptionNotHaveGymRooms");
}
