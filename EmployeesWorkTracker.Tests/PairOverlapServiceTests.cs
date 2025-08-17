using EmployeesWorkTracker.Application.Abstractions;
using EmployeesWorkTracker.Application.Services;
using EmployeesWorkTracker.Domain;

namespace EmployeesWorkTracker.Tests
{
    public class PairOverlapServiceTests
    {
        private static DateRange R(string from, string to)
        => new(DateOnly.Parse(from), DateOnly.Parse(to));

        private readonly IPairOverlapService _svc = new PairOverlapService();

        [Fact]
        public void ReturnsZeroResult_WhenNoOverlaps()
        {
            var data = new List<ProjectAssignment>
        {
            new(1, 100, R("2020-01-01", "2020-01-10")),
            new(2, 200, R("2020-02-01", "2020-02-10")),
        };

            var res = _svc.ComputeTopPair(data);

            Assert.Equal(0, res.TotalDaysWorkedTogether);
            Assert.Empty(res.Breakdown);
            Assert.Equal(0, res.EmployeeId1);
            Assert.Equal(0, res.EmployeeId2);
        }

        [Fact]
        public void ComputesInclusiveOverlap_OnSameProject()
        {
            var data = new List<ProjectAssignment>
        {
            new(1, 100, R("2020-01-01", "2020-01-10")),
            new(2, 100, R("2020-01-05", "2020-01-05")),
        };

            var res = _svc.ComputeTopPair(data);

            // 2020-01-05 срещу 2020-01-05 -> 1 ден (inclusive)
            Assert.Equal(1, res.TotalDaysWorkedTogether);
            Assert.Single(res.Breakdown);
            Assert.Equal(100, res.Breakdown[0].ProjectId);
            Assert.Equal(1, res.Breakdown[0].DaysWorked);
        }

        [Fact]
        public void AggregatesAcrossMultipleProjects()
        {
            var data = new List<ProjectAssignment>
        {
            new(1, 10, R("2020-01-01", "2020-01-10")),
            new(2, 10, R("2020-01-05", "2020-01-08")), // overlap 4 дни (5..8)
            new(1, 20, R("2020-02-01", "2020-02-10")),
            new(2, 20, R("2020-02-03", "2020-02-05")), // overlap 3 дни (3..5)
        };

            var res = _svc.ComputeTopPair(data);

            Assert.Equal(7, res.TotalDaysWorkedTogether);
            Assert.Equal(2, res.Breakdown.Count);
            Assert.Contains(res.Breakdown, x => x.ProjectId == 10 && x.DaysWorked == 4);
            Assert.Contains(res.Breakdown, x => x.ProjectId == 20 && x.DaysWorked == 3);
        }

        [Fact]
        public void TieBreaker_PicksHigherSingleProjectThenLowerIds()
        {
            var data = new List<ProjectAssignment>
        {
            // Pair (1,2): total 6 -> proj 10 = 6
            new(1, 10, R("2020-01-01", "2020-01-10")),
            new(2, 10, R("2020-01-05", "2020-01-10")), // 6 дни (5..10)

            // Pair (3,4): total 6 -> proj 30 = 5, proj 40 = 1  (max single = 5 < 6)
            new(3, 30, R("2020-03-01", "2020-03-05")),
            new(4, 30, R("2020-03-01", "2020-03-05")), // 5 дни
            new(3, 40, R("2020-04-01", "2020-04-01")),
            new(4, 40, R("2020-04-01", "2020-04-01")), // 1 ден
        };

            var res = _svc.ComputeTopPair(data);

            // Очакваме (1,2) заради по-големия single-project (6 срещу 5)
            Assert.Equal(1, res.EmployeeId1);
            Assert.Equal(2, res.EmployeeId2);
            Assert.Equal(6, res.TotalDaysWorkedTogether);
        }

        [Fact]
        public void HandlesMultipleIntervalsPerEmployeeOnSameProject()
        {
            var data = new List<ProjectAssignment>
        {
            new(1, 10, R("2020-01-01", "2020-01-03")),
            new(1, 10, R("2020-01-05", "2020-01-05")), // 2 интервала за Emp 1 на проект 10
            new(2, 10, R("2020-01-02", "2020-01-05")),
        };

            var res = _svc.ComputeTopPair(data);

            // Overlaps:
            // (1:1..3) ∩ (2:2..5) = 2 дни (2..3)
            // (1:5..5) ∩ (2:2..5) = 1 ден (5..5)
            Assert.Equal(3, res.TotalDaysWorkedTogether);
            Assert.Single(res.Breakdown);
            Assert.Equal(10, res.Breakdown[0].ProjectId);
            Assert.Equal(3, res.Breakdown[0].DaysWorked);
        }
    }
}

