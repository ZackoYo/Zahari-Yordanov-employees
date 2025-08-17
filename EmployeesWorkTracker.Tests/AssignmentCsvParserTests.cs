using EmployeesWorkTracker.Application.Abstractions;
using EmployeesWorkTracker.Application.Services;
using System.Text;

namespace EmployeesWorkTracker.Tests
{
    file sealed class FixedDateProvider : IDateProvider
    {
        public DateOnly Fixed { get; init; } = new DateOnly(2025, 01, 15);
        public DateOnly Today => Fixed;
    }

    public class AssignmentCsvParserTests
    {
        private static MemoryStream Ms(string content)
            => new MemoryStream(Encoding.UTF8.GetBytes(content));

        [Fact]
        public async Task Parses_WithHeader_AndVariousDateFormats()
        {
            // Различни формати + NULL + ; като разделител
            var csv = """
                EmpID, ProjectID, DateFrom, DateTo
                143, 10, 2013-11-01, 2014-01-05
                218; 10; 16/05/2012; NULL
                300, 20, 01.02.2020, 05.02.2020
                """;

            var parser = new AssignmentCsvParser(new FixedDateProvider());
            var res = await parser.ParseAsync(Ms(csv));

            Assert.Equal(3, res.Assignments.Count);
            Assert.Empty(res.Warnings);

            var a1 = res.Assignments[0];
            Assert.Equal(143, a1.EmployeeId);
            Assert.Equal(10, a1.ProjectId);
            Assert.Equal(new DateOnly(2013, 11, 01), a1.Range.From);
            Assert.Equal(new DateOnly(2014, 01, 05), a1.Range.To);

            var a2 = res.Assignments[1];
            Assert.Equal(218, a2.EmployeeId);
            Assert.Equal(10, a2.ProjectId);
            Assert.Equal(new DateOnly(2012, 05, 16), a2.Range.From);
            // NULL -> Fixed "Today"
            Assert.Equal(new DateOnly(2025, 01, 15), a2.Range.To);

            var a3 = res.Assignments[2];
            Assert.Equal(new DateOnly(2020, 02, 01), a3.Range.From);
            Assert.Equal(new DateOnly(2020, 02, 05), a3.Range.To);
        }

        [Fact]
        public async Task Skips_InvalidRows_ButContinues()
        {
            var csv = """
                EmpID,ProjectID,DateFrom,DateTo
                x, 10, 2020-01-01, 2020-01-10
                100, y, 2020-01-01, 2020-01-10
                100, 10, bad-date, 2020-01-10
                100, 10, 2020-01-01, bad-date
                100, 10, 2020-01-05, 2020-01-01
                """;

            var parser = new AssignmentCsvParser(new FixedDateProvider());
            var res = await parser.ParseAsync(Ms(csv));

            // Само последният ред е валиден (датите са разменени, но DateRange ще ги нормализира)
            Assert.Single(res.Assignments);
            var a = res.Assignments[0];
            Assert.Equal(100, a.EmployeeId);
            Assert.Equal(10, a.ProjectId);
            Assert.Equal(new DateOnly(2020, 01, 01), a.Range.From);
            Assert.Equal(new DateOnly(2020, 01, 05), a.Range.To);

            Assert.Equal(4, res.Warnings.Count);
        }

        [Fact]
        public async Task Accepts_NoHeader_And_CommaOnly()
        {
            var csv = """
                143,10,2024-01-01,2024-01-02
                144,10,2024-01-01,2024-01-03
                """;
            var parser = new AssignmentCsvParser(new FixedDateProvider());
            var res = await parser.ParseAsync(Ms(csv));

            Assert.Equal(2, res.Assignments.Count);
            Assert.Empty(res.Warnings);
        }
    }
}
