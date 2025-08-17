namespace EmployeesWorkTracker.Domain
{
    public sealed class ProjectAssignment
    {
        public int EmployeeId { get; }
        public int ProjectId { get; }
        public DateRange Range { get; }

        public ProjectAssignment(int employeeId, int projectId, DateRange range)
        {
            EmployeeId = employeeId;
            ProjectId = projectId;
            Range = range;
        }

        public override string ToString()
            => $"{EmployeeId}, {ProjectId}, {Range}";
    }
}
