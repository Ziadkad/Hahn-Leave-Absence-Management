namespace HahnLeaveAbsenceManagement.Application.Common.Models;

public record PageQueryRequest
{
    public int Page { get; set; } = 1;
    
    public int PageSize { get; set; } = 10;
    
}