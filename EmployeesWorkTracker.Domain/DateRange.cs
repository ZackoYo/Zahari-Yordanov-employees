namespace EmployeesWorkTracker.Domain
{
    public sealed class DateRange
    {
        public DateOnly From { get; }
        public DateOnly To { get; }

        public DateRange(DateOnly from, DateOnly to)
        {
            if (from <= to)
            {
                From = from;
                To = to;
            }
            else
            {
                To = from;
                From = to;
            }
        }

        public static int OverlapDaysInclusive(DateRange a, DateRange b)
        {
            var start = a.From > b.From ? a.From : b.From;
            var end = a.To < b.To ? a.To : b.To;

            if (start > end) return 0;

            return end.DayNumber - start.DayNumber + 1;
        }

        public override string ToString() => $"[{From:yyyy-MM-dd}..{To:yyyy-MM-dd}]";
    }
}
