namespace HMS.Appointment.Domain.Enums
{
    public enum ConflictType
    {
        DoubleBooking = 1,
        DoctorUnavailable = 2,
        RoomConflict = 3,
        OutsideWorkingHours = 4
    }
}
