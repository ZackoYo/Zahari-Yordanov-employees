using EmployeesWorkTracker.Api.Endpoints;
using EmployeesWorkTracker.Application;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => Results.Ok(new { status = "ok", service = "EmployeesWorkTracker.Api" }));

app.MapAnalyzeEndpoints();

app.Run();