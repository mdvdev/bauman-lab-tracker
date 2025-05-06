using LabTracker.Application.Contracts.Labs;
using LabTracker.Domain.Entities;
using LabTracker.Domain.ValueObjects;
using LabTracker.Presentation.Dtos;
using Microsoft.OpenApi.Models;

namespace LabTracker.Presentation;

public static class LabEndpoints
{
    public static IEndpointRouteBuilder MapLabEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var labGroup = endpoints.MapGroup("labs")
            .WithTags("Labs")
            .WithOpenApi();

        labGroup.MapGet("/", async (Guid courseId, ILabService labService) =>
        {
            var labs = await labService.GetLabsByCourseIdAsync(courseId);
            return Results.Ok(labs);
        });


        labGroup.MapPost("/", async (Guid courseId, CreateLabRequest request, ILabService labService) =>
            {
                var validationResult = ValidateLabRequest(request);
                if (validationResult is not null) return validationResult;

                try
                {
                    var lab = await labService.CreateLabAsync(
                        courseId,
                        request.Name,
                        request.Description,
                        request.Deadline,
                        request.Score,
                        request.ScoreAfterDeadline);

                    return Results.Created($"/api/courses/{courseId}/labs/{lab.Id}", lab);
                }
                catch (KeyNotFoundException ex)
                {
                    return Results.NotFound(ex.Message);
                }
            })
            .WithSummary("Создать лабораторную работу (преподаватель курса)");

        var labItemGroup = labGroup.MapGroup("/{labId}")
            .WithOpenApi();

        labItemGroup.MapGet("/", async (Guid labId, ILabService labService) =>
            {
                var lab = await labService.GetLabByIdAsync(labId);
                return lab is not null ? Results.Ok(lab) : Results.NotFound();
            })
            .WithSummary("Получить лабораторную работу (участник курса)");

        labItemGroup.MapPatch("/", async (Guid labId, UpdateLabRequest request, ILabService labService) =>
            {
                var validationResult = ValidateLabRequest(request);
                if (validationResult is not null) return validationResult;

                try
                {
                    await labService.UpdateLabAsync(
                        labId,
                        request.Name,
                        request.Description,
                        request.Deadline,
                        request.Score,
                        request.ScoreAfterDeadline);
                    
                    var updatedLab = await labService.GetLabByIdAsync(labId);
                    return Results.Ok(updatedLab);
                }
                catch (KeyNotFoundException ex)
                {
                    return Results.NotFound(ex.Message);
                }
            })
            .WithSummary("Обновить лабораторную работу (преподаватель курса)");

        return endpoints;
    }

    private static IResult? ValidateLabRequest(ILabRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return Results.BadRequest("Name is required");

        if (string.IsNullOrWhiteSpace(request.Description))
            return Results.BadRequest("Description is required");

        if (request.Score < 0 || request.ScoreAfterDeadline < 0)
            return Results.BadRequest("Score values cannot be negative");

        if (request.Deadline < DateTimeOffset.UtcNow)
            return Results.BadRequest("Deadline cannot be in the past");

        return null;
    }
}