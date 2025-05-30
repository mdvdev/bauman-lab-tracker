using LabTracker.Courses.Abstractions.Repositories;
using LabTracker.Labs.Abstractions.Repositories;
using LabTracker.Labs.Abstractions.Services;
using LabTracker.Labs.Abstractions.Services.Dtos;
using LabTracker.Labs.Domain;
using LabTracker.Shared.Contracts;

namespace Labs.Services;

public class LabService : ILabService
{
    private readonly ILabRepository _labRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IFileService _fileService;

    private const string SaveDirectory = "StaticFiles/Labs/Descriptions";

    public LabService(
        ILabRepository labRepository,
        ICourseRepository courseRepository,
        IFileService fileService)
    {
        _labRepository = labRepository;
        _courseRepository = courseRepository;
        _fileService = fileService;
    }

    public async Task<Lab> CreateLabAsync(Guid courseId, CreateLabRequest request)
    {
        if (await _courseRepository.GetByIdAsync(courseId) is null)
            throw new KeyNotFoundException($"Course with id '{courseId}' not found.");

        var lab = Lab.CreateNew(
            courseId: courseId,
            name: request.Name,
            descriptionUri: null,
            deadline: request.Deadline,
            score: request.Score,
            scoreAfterDeadline: request.ScoreAfterDeadline);

        await _labRepository.CreateAsync(lab);
        return lab;
    }

    public async Task DeleteLabAsync(Guid labId)
    {
        if (await _labRepository.GetByIdAsync(labId) is null)
            throw new KeyNotFoundException($"Lab with '{labId}' not found.");

        await _labRepository.DeleteAsync(labId);
    }

    public async Task<Lab?> GetLabByIdAsync(Guid labId)
    {
        return await _labRepository.GetByIdAsync(labId);
    }

    public async Task<IEnumerable<Lab>> GetLabsByCourseIdAsync(Guid courseId)
    {
        if (await _courseRepository.GetByIdAsync(courseId) is null)
            throw new KeyNotFoundException($"Course with '{courseId}' not found.");

        return await _labRepository.GetByCourseIdAsync(courseId);
    }

    public async Task UpdateLabAsync(Guid labId, UpdateLabRequest request)
    {
        var lab = await _labRepository.GetByIdAsync(labId)
                  ?? throw new KeyNotFoundException($"Lab with id '{labId}' not found.");

        lab.Update(
            name: request.Name,
            deadline: request.Deadline,
            score: request.Score,
            scoreAfterDeadline: request.ScoreAfterDeadline);

        await _labRepository.UpdateAsync(lab);
    }

    public async Task UpdateLabDescriptionAsync(Guid labId, Stream stream, string fileName)
    {
        var lab = await _labRepository.GetByIdAsync(labId);
        if (lab is null)
            throw new KeyNotFoundException($"Lab with id '{labId}' not found.");

        var filePath = await _fileService.SaveFileAsync(
            stream,
            SaveDirectory,
            fileName
        );

        if (lab.DescriptionUri is not null)
            _fileService.DeleteFile(lab.DescriptionUri.TrimStart('/'));

        lab.Update(descriptionUri: "/" + filePath);

        await _labRepository.UpdateAsync(lab);
    }
}