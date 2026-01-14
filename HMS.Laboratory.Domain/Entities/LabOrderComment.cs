namespace HMS.Laboratory.Domain.Entities
{
    public class LabOrderComment
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public string UserRole { get; set; } // Doctor, Technician, Nurse
        public string Comment { get; set; }
        public DateTime CommentDate { get; set; }
        public bool IsInternal { get; set; } // Internal notes vs patient-visible

        // Navigation
        public LabOrder Order { get; set; }
    }
}
