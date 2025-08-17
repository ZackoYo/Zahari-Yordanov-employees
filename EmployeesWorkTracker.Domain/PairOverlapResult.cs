namespace EmployeesWorkTracker.Domain
{
    public sealed class PairOverlapResult
    {
        public int EmployeeId1 { get; }
        public int EmployeeId2 { get; }
        public int TotalDaysWorkedTogether { get; }
        public IReadOnlyList<PairProjectContribution> Breakdown { get; }

        public PairOverlapResult(int employeeId1, int employeeId2, int totalDaysWorkedTogether,
            IReadOnlyList<PairProjectContribution> breakdown)
        {
            EmployeeId1 = employeeId1;
            EmployeeId2 = employeeId2;
            TotalDaysWorkedTogether = totalDaysWorkedTogether;
            Breakdown = breakdown;
        }

        public override string ToString()
            => $"{EmployeeId1}, {EmployeeId2}, {TotalDaysWorkedTogether}";
    }
}
