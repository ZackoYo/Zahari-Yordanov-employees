using EmployeesWorkTracker.Application.Abstractions;

namespace EmployeesWorkTracker.Application.Services
{
    public sealed class SystemDateProvider : IDateProvider
    {
        public DateOnly Today => DateOnly.FromDateTime(DateTime.UtcNow);
    }
}
