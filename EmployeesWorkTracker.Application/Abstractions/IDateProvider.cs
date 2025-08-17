namespace EmployeesWorkTracker.Application.Abstractions
{
    public interface IDateProvider
    {
        DateOnly Today { get; }
    }
}
