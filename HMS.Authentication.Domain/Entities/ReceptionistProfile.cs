namespace HMS.Authentication.Domain.Entities
{
    public class ReceptionistProfile : BaseStaffProfile
    {
        public string? Shift { get; set; }
        public string? Desk { get; set; }
    }
}
