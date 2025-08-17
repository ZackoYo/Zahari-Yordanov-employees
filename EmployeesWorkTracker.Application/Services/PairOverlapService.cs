using EmployeesWorkTracker.Application.Abstractions;
using EmployeesWorkTracker.Domain;

namespace EmployeesWorkTracker.Application.Services
{
    public sealed class PairOverlapService : IPairOverlapService
    {
        public PairOverlapResult ComputeTopPair(IEnumerable<ProjectAssignment> assignments)
        {
            var list = assignments?.ToList() ?? new List<ProjectAssignment>();

            var totalsByPair = new Dictionary<PairKey, int>();

            var breakdownByPair = new Dictionary<PairKey, Dictionary<int, int>>();

            foreach (var group in list.GroupBy(x => x.ProjectId))
            {
                var projId = group.Key;
                var projectAssignments = group.ToList();

                for (int i = 0; i < projectAssignments.Count; i++)
                {
                    for (int j = i + 1; j < projectAssignments.Count; j++)
                    {
                        var x = projectAssignments[i];
                        var y = projectAssignments[j];

                        if (x.EmployeeId == y.EmployeeId) continue;

                        var pairKey = new PairKey(x.EmployeeId, y.EmployeeId).Normalize();

                        var days = DateRange.OverlapDaysInclusive(x.Range, y.Range);
                        if (days <= 0) continue;

                        if (!totalsByPair.TryGetValue(pairKey, out var total))
                        {
                            totalsByPair[pairKey] = days;
                        }
                        else
                        {
                            totalsByPair[pairKey] = total + days;
                        }

                        if (!breakdownByPair.TryGetValue(pairKey, out var perProject))
                        {
                            perProject = new Dictionary<int, int>();
                            breakdownByPair[pairKey] = perProject;
                        }

                        if (!perProject.TryGetValue(projId, out var byProject))
                        {
                            perProject[projId] = days;
                        }
                        else
                        {
                            perProject[projId] = byProject + days;
                        }
                    }
                }
            }

            if (totalsByPair.Count == 0)
            {
                return new PairOverlapResult(0, 0, 0, Array.Empty<PairProjectContribution>());
            }

            var bestPair = totalsByPair.Keys
                .Select(k => new
                {
                    Key = k,
                    Total = totalsByPair[k],
                    MaxProject = breakdownByPair.TryGetValue(k, out var perProj) && perProj.Count > 0
                        ? perProj.Values.Max()
                        : 0
                })
                .OrderByDescending(x => x.Total)
                .ThenByDescending(x => x.MaxProject)
                .ThenBy(x => x.Key.EmployeeId1)
                .ThenBy(x => x.Key.EmployeeId2)
                .First();

            var bestBreakdown = breakdownByPair[bestPair.Key]
                .OrderByDescending(kv => kv.Value)
                .ThenBy(kv => kv.Key)
                .Select(kv => new PairProjectContribution(kv.Key, kv.Value))
                .ToList();

            return new PairOverlapResult(
                bestPair.Key.EmployeeId1,
                bestPair.Key.EmployeeId2,
                bestPair.Total,
                bestBreakdown
            );
        }
    }
}
