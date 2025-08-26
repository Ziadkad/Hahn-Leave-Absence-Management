namespace HahnLeaveAbsenceManagement.Application.Common.Exceptions;

public class DuplicateException(string entity)
    : Exception($"the entity '{entity}' already exists.");
