namespace EmployeesWorkTracker.Domain
{
    public sealed class CsvParseResult
    {
        public IReadOnlyList<ProjectAssignment> Assignments { get; }
        public IReadOnlyList<string> Warnings { get; }

        public CsvParseResult(IReadOnlyList<ProjectAssignment> assignments, IReadOnlyList<string> warnings)
        {
            Assignments = assignments;
            Warnings = warnings;
        }
    }
}
