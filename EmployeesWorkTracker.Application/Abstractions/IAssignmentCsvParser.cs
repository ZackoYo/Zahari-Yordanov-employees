using EmployeesWorkTracker.Domain;

namespace EmployeesWorkTracker.Application.Abstractions
{
    public interface IAssignmentCsvParser
    {
        Task<CsvParseResult> ParseAsync(Stream csvStream, CancellationToken ct = default);
    }
}
