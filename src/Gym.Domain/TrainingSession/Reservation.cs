namespace Gym.Domain;

public class Reservation: Entity
{
    public Guid ParticipantId { get; }

    public Reservation(Guid participantId, Guid? id = null) : base(id)
    {
        ParticipantId = participantId;
    }
}
