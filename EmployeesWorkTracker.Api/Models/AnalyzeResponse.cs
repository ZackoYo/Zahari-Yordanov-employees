namespace EmployeesWorkTracker.Api.Models
{
    public sealed class AnalyzeResponse
    {
        public int EmployeeId1 { get; init; }
        public int EmployeeId2 { get; init; }
        public int TotalDaysWorkedTogether { get; init; }
        public IReadOnlyList<ProjectBreakdownDto> Breakdown { get; init; } = Array.Empty<ProjectBreakdownDto>();
        public IReadOnlyList<string> Warnings { get; init; } = Array.Empty<string>();
    }
}
