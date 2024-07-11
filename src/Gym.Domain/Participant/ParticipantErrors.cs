using ErrorOr;

namespace Gym.Domain;

public class ParticipantErrors
{
    public static readonly Error TrainingSessionNotFind = Error.Validation(
        code: "Participant_001", "TrainingSessionNotFind");

    public static readonly Error TrainingSessionOverlappingReservedSession = Error.Validation(
        code: "Participant_002", "TrainingSessionOverlappingReservedSession");
}
