using ErrorOr;

namespace Gym.Domain;

public static class TrainingSessionErrors
{
    public static readonly Error UserIsEmpty = Error.Validation(
        code: "TrainingSession_001", "UserIsEmpty");

    public static readonly Error OnlyTrainerHaveAccessToChangeSessionState = Error.Validation(
        code: "TrainingSession_002", "OnlyTrainerHaveAccessToChangeSessionState");

    public static readonly Error NotHaveAvailableSpot = Error.Validation(
        code: "TrainingSession_003", "NotHaveAvailableSpot");

    public static readonly Error UserAlreadyReserveSpot = Error.Validation(
        code: "TrainingSession_004", "UserAlreadyReserveSpot");

    public static readonly Error SessionAlreadyEnd = Error.Validation(
        code: "TrainingSession_005", "SessionAlreadyEnd");

    public static readonly Error YouLate = Error.Validation(
        code: "TrainingSession_006", "YouLate");

    public static readonly Error YouNotResponseForThisSession = Error.Validation(
        code: "TrainingSession_007", "YouNotResponseForThisSession");

    public static readonly Error CannotHaveMoreSessionsThanSubscritpionAllows = Error.Validation(
        code: "TrainingSession_008", "CannotHaveMoreSessionsThanSubscritpionAllows");

    public static readonly Error SubscriptionNotHaveSessions = Error.Validation(
        code: "TrainingSession_009", "SubscriptionNotHaveSessions");
}
