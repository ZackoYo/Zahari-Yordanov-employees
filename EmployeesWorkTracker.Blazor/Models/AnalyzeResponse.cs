namespace EmployeesWorkTracker.Blazor.Models
{
    public sealed class AnalyzeResponse
    {
        public int EmployeeId1 { get; set; }
        public int EmployeeId2 { get; set; }
        public int TotalDaysWorkedTogether { get; set; }
        public List<ProjectBreakdownDto> Breakdown { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
    }
}
