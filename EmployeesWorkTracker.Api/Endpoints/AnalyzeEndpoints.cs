using EmployeesWorkTracker.Api.Models;
using EmployeesWorkTracker.Application.Abstractions;
using EmployeesWorkTracker.Domain;
using Microsoft.AspNetCore.Http.HttpResults;

namespace EmployeesWorkTracker.Api.Endpoints;

public static class AnalyzeEndpoints
{
    public static IEndpointRouteBuilder MapAnalyzeEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/assignments/analyze",
            async Task<Results<Ok<AnalyzeResponse>, BadRequest<string>>> (
                IFormFile file,
                IAssignmentCsvParser parser,
                IPairOverlapService overlapService,
                CancellationToken ct) =>
            {
                if (file is null || file.Length == 0)
                    return TypedResults.BadRequest("Missing file or file is empty.");

                if (!IsCsv(file.FileName))
                    return TypedResults.BadRequest("A .csv file is required.");

                using var stream = file.OpenReadStream();
                var parse = await parser.ParseAsync(stream, ct);

                PairOverlapResult top = overlapService.ComputeTopPair(parse.Assignments);

                var resp = new AnalyzeResponse
                {
                    EmployeeId1 = top.EmployeeId1,
                    EmployeeId2 = top.EmployeeId2,
                    TotalDaysWorkedTogether = top.TotalDaysWorkedTogether,
                    Breakdown = top.Breakdown
                        .Select(b => new ProjectBreakdownDto { ProjectId = b.ProjectId, DaysWorked = b.DaysWorked })
                        .ToList(),
                    Warnings = parse.Warnings.ToList()
                };

                return TypedResults.Ok(resp);
            })
            .DisableAntiforgery()
            .Accepts<IFormFile>("multipart/form-data")
            .Produces<AnalyzeResponse>(StatusCodes.Status200OK)
            .Produces<string>(StatusCodes.Status400BadRequest)
            .WithName("AnalyzeAssignments")
            .WithSummary("Analyzes a CSV with employees/projects and returns the top pair with breakdown.")
            .WithDescription("Upload a .csv (multipart/form-data, field: file). Supports multiple date formats and NULL for DateTo.");

        return app;
    }

    private static bool IsCsv(string fileName)
        => Path.GetExtension(fileName).Equals(".csv", StringComparison.OrdinalIgnoreCase);
}