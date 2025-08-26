namespace HahnLeaveAbsenceManagement.Application.Common.Exceptions;

public class ForbiddenException()
    : Exception("The current user is not allowed to perform this action");