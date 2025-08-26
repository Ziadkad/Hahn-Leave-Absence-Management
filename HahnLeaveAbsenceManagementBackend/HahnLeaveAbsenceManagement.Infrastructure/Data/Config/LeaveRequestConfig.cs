using HahnLeaveAbsenceManagement.Domain.LeaveRequest;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HahnLeaveAbsenceManagement.Infrastructure.Data.Config;

public class LeaveRequestConfig: IEntityTypeConfiguration<LeaveRequest>
{
    public void Configure(EntityTypeBuilder<LeaveRequest> builder)
    {
        builder.ToTable("LeaveRequests");

        builder.HasKey(lr => lr.Id);

        builder.Property(lr => lr.Type)
            .IsRequired();
        
        builder.Property(lr => lr.StartDate)
            .IsRequired();
        
        builder.Property(lr => lr.EndDate)
            .IsRequired();
        
        builder.Property(lr => lr.Status)
            .IsRequired();
        
        builder.Property(lr => lr.BusinessDays)
            .IsRequired();
        
        builder.Property(lr => lr.UserId)
            .IsRequired();
    }
}