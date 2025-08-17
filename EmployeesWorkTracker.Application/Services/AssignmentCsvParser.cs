using EmployeesWorkTracker.Application.Abstractions;
using EmployeesWorkTracker.Domain;
using System.Globalization;

namespace EmployeesWorkTracker.Application.Services
{
    public sealed class AssignmentCsvParser : IAssignmentCsvParser
    {
        private static readonly string[] DateFormats =
        {
            "yyyy-MM-dd", "yyyy/M/d", "yyyy.M.d",
            "dd-MM-yyyy", "d-M-yyyy", "dd.MM.yyyy", "d.M.yyyy",
            "dd/MM/yyyy", "d/M/yyyy",
            "MM/dd/yyyy", "M/d/yyyy",
            "MMM d, yyyy", "d MMM yyyy"
        };

        private readonly IDateProvider _dateProvider;

        public AssignmentCsvParser(IDateProvider dateProvider)
        {
            _dateProvider = dateProvider;
        }

        public async Task<CsvParseResult> ParseAsync(Stream csvStream, CancellationToken ct = default)
        {
            var assignments = new List<ProjectAssignment>();
            var warnings = new List<string>();

            using var reader = new StreamReader(csvStream, leaveOpen: true);

            int lineNo = 0;
            bool headerChecked = false;

            while (!reader.EndOfStream)
            {
                ct.ThrowIfCancellationRequested();
                var raw = await reader.ReadLineAsync();
                lineNo++;

                if (string.IsNullOrWhiteSpace(raw))
                {
                    continue;
                }

                var parts = SplitSmart(raw);

                if (!headerChecked)
                {
                    headerChecked = true;
                    if (parts.Length >= 4 && !int.TryParse(parts[0], out _))
                    {
                        continue; 
                    }
                }

                if (parts.Length < 4)
                {
                    warnings.Add($"Line {lineNo}: insufficient columns (expected >= 4). Skipped.");
                    continue;
                }

                if (!int.TryParse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out var empId))
                {
                    warnings.Add($"Line {lineNo}: invalid EmpID '{parts[0]}'. Skipped.");
                    continue;
                }

                if (!int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out var projId))
                {
                    warnings.Add($"Line {lineNo}: invalid ProjectID '{parts[1]}'. Skipped.");
                    continue;
                }

                var dateFromStr = parts[2];
                var dateToStr = parts[3];

                if (!TryParseDate(dateFromStr, out var from))
                {
                    warnings.Add($"Line {lineNo}: invalid DateFrom '{dateFromStr}'. Skipped.");
                    continue;
                }

                DateOnly to;
                if (IsNull(dateToStr))
                {
                    to = _dateProvider.Today;
                }
                else if (!TryParseDate(dateToStr, out to))
                {
                    warnings.Add($"Line {lineNo}: invalid DateTo '{dateToStr}'. Skipped.");
                    continue;
                }

                var range = new DateRange(from, to);
                assignments.Add(new ProjectAssignment(empId, projId, range));
            }

            return new CsvParseResult(assignments, warnings);
        }

        private static bool IsNull(string? s)
            => string.IsNullOrWhiteSpace(s) || s.Trim().Equals("NULL", StringComparison.OrdinalIgnoreCase);

        private static string[] SplitSmart(string line)
        {
            char delimiter = line.Contains(';') ? ';' : ',';
            return line.Split(delimiter)
                .Select(x => x.Trim())
                .ToArray();
        }

        private static bool TryParseDate(string input, out DateOnly date)
        {
            if (DateOnly.TryParseExact(input, DateFormats, CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out date))
                return true;

            if (DateTime.TryParse(input, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dtInv))
            {
                date = DateOnly.FromDateTime(dtInv);
                return true;
            }

            if (DateTime.TryParse(input, CultureInfo.CurrentCulture, DateTimeStyles.None, out var dtLoc))
            {
                date = DateOnly.FromDateTime(dtLoc);
                return true;
            }

            date = default;
            return false;
        }
    }
}
