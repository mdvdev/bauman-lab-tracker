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
        var labGroup = endpoints.MapGroup("/api/courses/{courseId}/labs")
            .WithTags("Labs")
            .WithOpenApi();

        labGroup.MapGet("/", async (Guid courseId, ILabService labService) =>
            {
                var labs = await labService.GetLabsByCourseIdAsync(courseId);
                return Results.Ok(labs);
            })
            .RequireAuthorization(policy => policy
                .RequireRole(nameof(Role.Teacher), nameof(Role.Student), nameof(Role.Administrator)))
            .Produces<List<Lab>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .WithSummary("Получить список лабораторных работ (участник курса)")
            .WithOpenApi(operation => {
                operation.Security = new List<OpenApiSecurityRequirement>
                {
                    new()
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            Array.Empty<string>()
                        }
                    }
                };
                return operation;
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
            .RequireAuthorization(policy => policy
                .RequireRole(nameof(Role.Teacher), nameof(Role.Administrator)))
            .Produces<Lab>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .WithSummary("Создать лабораторную работу (преподаватель курса)")
            .WithOpenApi(operation => {
                operation.Security = new List<OpenApiSecurityRequirement>
                {
                    new()
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            Array.Empty<string>()
                        }
                    }
                };
                operation.RequestBody = new OpenApiRequestBody
                {
                    Content =
                    {
                        ["application/json"] = new OpenApiMediaType
                        {
                            Schema = new OpenApiSchema
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.Schema,
                                    Id = "LabCreate"
                                }
                            }
                        }
                    }
                };
                return operation;
            });

        var labItemGroup = labGroup.MapGroup("/{labId}")
            .WithOpenApi();

        labItemGroup.MapGet("/", async (Guid labId, ILabService labService) =>
            {
                var lab = await labService.GetLabByIdAsync(labId);
                return lab is not null ? Results.Ok(lab) : Results.NotFound();
            })
            .RequireAuthorization(policy => policy
                .RequireRole(nameof(Role.Teacher), nameof(Role.Student), nameof(Role.Administrator)))
            .Produces<Lab>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .WithSummary("Получить лабораторную работу (участник курса)")
            .WithOpenApi(operation => {
                operation.Security = new List<OpenApiSecurityRequirement>
                {
                    new()
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            Array.Empty<string>()
                        }
                    }
                };
                operation.Parameters[0].Description = "ID курса";
                operation.Parameters[1].Description = "ID лабораторной работы";
                return operation;
            });

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
            .RequireAuthorization(policy => policy
                .RequireRole(nameof(Role.Teacher), nameof(Role.Administrator)))
            .Produces<Lab>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .WithSummary("Обновить лабораторную работу (преподаватель курса)")
            .WithOpenApi(operation => {
                operation.Security = new List<OpenApiSecurityRequirement>
                {
                    new()
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            Array.Empty<string>()
                        }
                    }
                };
                operation.RequestBody = new OpenApiRequestBody
                {
                    Content =
                    {
                        ["application/json"] = new OpenApiMediaType
                        {
                            Schema = new OpenApiSchema
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.Schema,
                                    Id = "LabUpdate"
                                }
                            }
                        }
                    }
                };
                return operation;
            });

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