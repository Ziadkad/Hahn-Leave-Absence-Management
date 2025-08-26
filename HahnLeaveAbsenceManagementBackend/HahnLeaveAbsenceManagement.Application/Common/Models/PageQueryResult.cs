namespace HahnLeaveAbsenceManagement.Application.Common.Models;

public record PageQueryResult<T>(IEnumerable<T> Items, in int TotalCount)
{
    public IEnumerable<T> Items { get; set; } = Items;

    public int TotalCount { get; set; } = TotalCount;
}