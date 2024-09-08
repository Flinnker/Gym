using ErrorOr;

namespace Gym.Domain;

public static class TimeRangeErrors {

    public static readonly Error NotAvailableSessionBeginTime = Error.Validation(
        code: "TimeRange_001", "NotAvailableSessionBeginTime");

    public static readonly Error NotAvailableSessionEndTime = Error.Validation(
        code: "TimeRange_002", "NotAvailableSessionEndTime");

    public static readonly Error EndCanNotBeEarlierThenBegin = Error.Validation(
        code: "TimeRange_003", "EndCanNotBeEarlierThenBegin");

    public static readonly Error DurationCanNotBeZeroMinutes = Error.Validation(
        code: "TimeRange_004", "DurationCanNotBeZeroMinutes");
}
