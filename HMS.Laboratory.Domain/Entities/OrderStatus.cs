namespace HMS.Laboratory.Domain.Entities
{
    public enum OrderStatus
    {
        Pending = 1,
        Ordered = 2,
        SampleCollected = 3,
        SampleReceived = 4,
        InProgress = 5,
        Completed = 6,
        Verified = 7,
        Reported = 8,
        Cancelled = 9,
        Failed = 10,
        Rejected = 11,
        Hold = 12,
        Stat = 13,
        Preliminary = 14,
        Final = 15,
        Amended = 16,
        Transcribed = 17
    }
}
