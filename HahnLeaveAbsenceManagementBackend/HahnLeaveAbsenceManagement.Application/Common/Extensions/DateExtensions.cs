namespace HahnLeaveAbsenceManagement.Application.Common.Extensions;

public static class DateExtensions
{
    public static int GetBusinessDays(DateTime startDate, DateTime endDate)
    {
        int totalDays = (endDate - startDate).Days + 1; // +1 to include start day
        int businessDays = 0;

        for (int i = 0; i < totalDays; i++)
        {
            var currentDay = startDate.AddDays(i);
            if (currentDay.DayOfWeek != DayOfWeek.Saturday &&
                currentDay.DayOfWeek != DayOfWeek.Sunday)
            {
                businessDays++;
            }
        }

        return businessDays;
    }
}