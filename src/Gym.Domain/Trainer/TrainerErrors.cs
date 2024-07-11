using ErrorOr;

namespace Gym.Domain;

public static class TrainerErrors
{
    public static readonly Error UserMustBeTrainerInThisSession = Error.Validation(
        code: "Trainer_001", "UserMustBeTrainerInThisSession");

    public static readonly Error TrainerAlreadyHaveThisSession = Error.Validation(
        code: "Trainer_002", "TrainerAlreadyHaveThisSession");

    public static readonly Error TrainerAlreadyNotHaveThisSession = Error.Validation(
        code: "Trainer_003", "TrainerAlreadyNotHaveThisSession");
}
