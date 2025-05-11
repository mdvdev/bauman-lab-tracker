using LabTracker.Application.Contracts;
using LabTracker.Domain.Entities;
using LabTracker.Domain.ValueObjects;

namespace LabTracker.Application.Courses.Core;

public class CourseService : ICourseService
{
    private readonly ICourseRepository _courseRepository;
    private readonly IFileService _fileService;
    private const string SaveDirectory = "StaticFiles/Images/CoursePhotos";

    public CourseService(ICourseRepository courseRepository, IFileService fileService)
    {
        _courseRepository = courseRepository;
        _fileService = fileService;
    }

    public async Task<Guid> CreateCourseAsync(CreateCourseCommand command)
    {
        var course = new Course
        {
            Name = command.Name,
            Description = command.Description,
            QueueMode = command.QueueMode
        };
        return await _courseRepository.CreateAsync(course);
    }

    public async Task<Course?> GetCourseDetailsAsync(Guid courseId)
    {
        return await _courseRepository.GetByIdAsync(courseId);
    }

    public async Task UpdateCourseAsync(Guid courseId, UpdateCourseCommand command)
    {
        var course = await _courseRepository.GetByIdAsync(courseId);
        if (course is null)
            throw new KeyNotFoundException($"Course with id {courseId} not found");

        if (command.Name is not null) course.Name = command.Name;
        if (command.Description is not null) course.Description = command.Description;
        if (command.QueueMode is not null) course.QueueMode = (QueueMode)command.QueueMode;

        await _courseRepository.UpdateAsync(course);
    }

    public async Task UpdateCoursePhotoAsync(Guid courseId, Stream stream, string fileName)
    {
        var course = await _courseRepository.GetByIdAsync(courseId);
        if (course is null)
            throw new KeyNotFoundException($"Course with id '{courseId}' not found.");

        var filePath = await _fileService.SaveFileAsync(
            stream,
            SaveDirectory,
            fileName
        );

        if (course.PhotoUri is not null)
            _fileService.DeleteFile(course.PhotoUri.TrimStart('/'));

        course.PhotoUri = "/" + filePath;
        await _courseRepository.UpdateAsync(course);
    }

    public async Task DeleteCourseAsync(Guid courseId)
    {
        if (await _courseRepository.GetByIdAsync(courseId) is null)
            throw new KeyNotFoundException($"Course with '{courseId}' not found.");

        await _courseRepository.DeleteAsync(courseId);
    }
}