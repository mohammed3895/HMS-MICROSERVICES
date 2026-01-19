namespace HMS.Web.Models.DTOs.Billing
{
    public class BillingDto
    {
        public Guid BillingId { get; set; }
        public Guid PatientId { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public DateTime BillingDate { get; set; }
    }
}
