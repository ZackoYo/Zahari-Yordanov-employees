namespace EmployeesWorkTracker.Domain
{
    public sealed class PairProjectContribution
    {
        public int ProjectId { get; }
        public int DaysWorked { get; }

        public PairProjectContribution(int projectId, int daysWorked)
        {
            ProjectId = projectId;
            DaysWorked = daysWorked;
        }
    }
}
