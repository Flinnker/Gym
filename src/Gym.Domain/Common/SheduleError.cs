using ErrorOr;

namespace Gym.Domain;

public static class SheduleError
{
    public static readonly Error OverlappReservedTrainingSession = Error.Validation(
        code: "Subscription_001", "OverlappReservedTrainingSession");

    public static readonly Error TrainingSessionNotFound = Error.Validation(
        code: "Subscription_002", "TrainingSessionNotFound");
}
