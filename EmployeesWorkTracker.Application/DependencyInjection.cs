using EmployeesWorkTracker.Application.Abstractions;
using EmployeesWorkTracker.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeesWorkTracker.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddSingleton<IDateProvider, SystemDateProvider>();
            services.AddSingleton<IPairOverlapService, PairOverlapService>();
            services.AddSingleton<IAssignmentCsvParser, AssignmentCsvParser>();
            return services;
        }
    }
}
