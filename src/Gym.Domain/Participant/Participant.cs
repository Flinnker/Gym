using ErrorOr;

namespace Gym.Domain;

public class Participant : Entity
{
    private List<Guid> _reservedTrainingSessions = [];
    private List<Guid> _availableForReserveTrainingSessions = [];
    private Shedule _shedule;

    public Participant(Shedule? shedule = null, Guid? id = null) : base(id)
    {
        _shedule = shedule ?? Shedule.CreateShedule();
    }

    public static Participant CreateParticipant() 
    {
        return new Participant();
    }

    public List<Guid> GetParticipantTrainingSessionsId() 
    {
        return _reservedTrainingSessions;
    }

    public ErrorOr<Success> ReserveTrainingSession(DateOnly startDate, TimeRange timeRange) 
    {
        if (_shedule.AddTrainingSessionAtShedule(startDate, timeRange) != Result.Success) 
        {
            return ParticipantErrors.TrainingSessionOverlappingReservedSession;
        }

        return Result.Success;
    }

    public ErrorOr<Success> CancelTrainingSession(DateOnly startDate, TimeRange timeRange) 
    {
        return _shedule.RemoveTrainingSessionFromShedule(startDate, timeRange);
    }
}
