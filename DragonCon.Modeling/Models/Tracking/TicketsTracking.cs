using NodaTime;

namespace DragonCon.Modeling.Models.Tracking
{
    public class TicketsTracking
    {
        public string Id { get; set; }
        public string TicketId { get; set; }
        public Instant TimeStamp { get; set; }
        public string ParticipantId { get;set; }
        public string ExecutorId { get;set; }
        public bool IsSelf => ParticipantId == ExecutorId;
        public TicketTrackAction Action { get; set; }
    }
}
