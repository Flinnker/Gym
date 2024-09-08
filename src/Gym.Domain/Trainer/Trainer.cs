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

        var addTrainingSessionResult = Shedule.AddTrainingSessionAtShedule(startDate, timeRange);

        if (addTrainingSessionResult == Result.Success) 
        {
            _trainingSessions.Add(addingSessionId);
        }

        return addTrainingSessionResult;
    }

    public ErrorOr<Success> RemoveSessionAtShedule(Guid removingSessionId, DateOnly startDate, TimeRange timeRange)
    {
        if (!_trainingSessions.Any(trainingSessionId => trainingSessionId == removingSessionId))
        {
            return TrainerErrors.TrainerAlreadyNotHaveThisSession;
        }

        var removeTrainingSessionResult = Shedule.RemoveTrainingSessionFromShedule(startDate, timeRange);

        if (removeTrainingSessionResult == Result.Success)
        {
            _trainingSessions.RemoveAll(sessionId => sessionId.Equals(removingSessionId));
        }

        return removeTrainingSessionResult;
    }

    public bool GetTrainerIsFree(DateOnly startDate, TimeRange timeRange)
    {
        return Shedule.GetTimeRangeIsFree(startDate, timeRange);
    }
}
