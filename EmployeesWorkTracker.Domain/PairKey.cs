namespace EmployeesWorkTracker.Domain
{
    public readonly record struct PairKey(int EmployeeId1, int EmployeeId2)
    {
        public PairKey Normalize()
        {
            return EmployeeId1 <= EmployeeId2
                ? this
                : new PairKey(EmployeeId2, EmployeeId1);
        }

        public override string ToString() => $"{EmployeeId1}-{EmployeeId2}";
    }
}
