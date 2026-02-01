using HMS.Common.DTOs;
using HMS.Staff.Application.DTOs;
using MediatR;

namespace HMS.Staff.Application.Queries
{
    public class GetDepartmentStatisticsQuery : IRequest<Result<DepartmentStatisticsDto>>
    {
        public string Department { get; set; } = string.Empty;
    }
}
