using EmployeesWorkTracker.Domain;

namespace EmployeesWorkTracker.Application.Abstractions
{
    public interface IPairOverlapService
    {
        PairOverlapResult ComputeTopPair(IEnumerable<ProjectAssignment> assignments);
    }
}
