using ErrorOr;

namespace Gym.Domain;

public class Trainer : Entity
{
    public Shedule Shedule { get; private set; }
    public List<Guid> _trainingSessions = [];

    public Trainer(Shedule? shedule = null, Guid? id = null) : base(id) 
    {
        Shedule = shedule ?? Shedule.CreateShedule();
    }

    public static Trainer CreateTrainer() 
    {
        return new Trainer();
    }

    public ErrorOr<Success> AddSessionAtShedule(Guid addingSessionId, DateOnly startDate, TimeRange timeRange) 
    {
        if (_trainingSessions.Any(trainingSessionId => trainingSessionId == addingSessionId)) 
        {
            return TrainerErrors.TrainerAlreadyHaveThisSession;
        }

        return Shedule.AddTrainingSessionAtShedule(startDate, timeRange);
    }

    public ErrorOr<Success> RemoveSessionAtShedule(Guid addingSessionId, DateOnly startDate, TimeRange timeRange)
    {
        if (!_trainingSessions.Any(trainingSessionId => trainingSessionId == addingSessionId))
        {
            return TrainerErrors.TrainerAlreadyNotHaveThisSession;
        }

        return Shedule.RemoveTrainingSessionFromShedule(startDate, timeRange);
    }

    public bool GetTrainerIsFree(DateOnly startDate, TimeRange timeRange)
    {
        return Shedule.GetTimeRangeIsFree(startDate, timeRange);
    }
}
