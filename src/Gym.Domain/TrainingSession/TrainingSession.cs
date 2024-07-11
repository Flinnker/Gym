using ErrorOr;

namespace Gym.Domain;

public class TrainingSession : Entity
{
    private Guid _gymRoomId;
    private List<Reservation> _reservations = [];
    private int _sessionSize;

    private TimeOnly _startSessionTime;
    private TimeOnly _endSessionTime;

    public TimeRange TimeRange { get; private set; }
    public DateOnly StartSessionDate { get; private set; }
    public Guid TrainerId { get; private set; }

    public TrainingSession(
        Guid gymRoomId, 
        int sessionSize, 
        Guid trainerId, 
        Guid? id = null) : base(id)
    {
        _gymRoomId = gymRoomId;
        _sessionSize = sessionSize;
        TrainerId = trainerId;
        TimeRange = TimeRange.Create(
            _startSessionTime, 
            _endSessionTime);
    }

    public ErrorOr<Success> ReserveSpot(Guid participantId) 
    {
        if (_reservations.Count >= _sessionSize) 
        {
            return TrainingSessionErrors.NotHaveAvailableSpot;
        }

        if (_reservations.Any(id => id.Equals(participantId)))
        {
            return TrainingSessionErrors.UserAlreadyReserveSpot;
        }

        _reservations.Add(new Reservation(participantId));

        return Result.Success;
    }

    public ErrorOr<Success> CancelReservation(Guid participantId, IDateTimeProvider dateTimeProvider)
    {
        if (!_reservations.Any(id => id.Equals(participantId)))
        {
            return TrainingSessionErrors.UserAlreadyReserveSpot;
        }

        if (TimeRange.GetSessionAlreadyEnd(StartSessionDate, dateTimeProvider)) 
        {
            return TrainingSessionErrors.SessionAlreadyEnd;
        }

        if (TimeRange.GetUserLateForCancel(StartSessionDate, dateTimeProvider))
        {
            return TrainingSessionErrors.YouLate;
        }

        _reservations.RemoveAll(item => item.ParticipantId.Equals(participantId));

        return Result.Success;
    }
}
