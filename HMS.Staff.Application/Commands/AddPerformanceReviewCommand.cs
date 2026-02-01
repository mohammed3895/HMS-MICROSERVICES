using HMS.Common.DTOs;
using MediatR;

namespace HMS.Staff.Application.Commands
{

    public class AddPerformanceReviewCommand : IRequest<Result<Guid>>
    {
        public Guid StaffId { get; set; }
        public Guid ReviewerId { get; set; }
        public DateTime ReviewDate { get; set; }
        public string ReviewPeriod { get; set; } = string.Empty;
        public int OverallRating { get; set; }
        public string? Strengths { get; set; }
        public string? AreasForImprovement { get; set; }
        public string? Goals { get; set; }
        public string? Comments { get; set; }
    }
}
