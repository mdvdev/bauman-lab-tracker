using LabTracker.Courses.Abstractions.Repositories;
using LabTracker.Courses.Abstractions.Services;
using LabTracker.Courses.Abstractions.Services.Dtos;
using LabTracker.Courses.Domain;
using LabTracker.Shared.Contracts;

namespace Courses.Services;

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

    public async Task<Course> CreateCourseAsync(CreateCourseRequest request)
    {
        var course = Course.CreateNew(
            name: request.Name,
            description: request.Description,
            queueMode: request.QueueMode);

        return await _courseRepository.CreateAsync(course);
    }

    public async Task<Course?> GetCourseDetailsAsync(Guid courseId)
    {
        return await _courseRepository.GetByIdAsync(courseId);
    }

    public async Task<IEnumerable<Course>> GetCoursesAsync()
    {
        return await _courseRepository.GetAllAsync();
    }

    public async Task<IEnumerable<Course>> GetUserCoursesAsync(Guid userId)
    {
        return await _courseRepository.GetCoursesByUserIdAsync(userId);
    }

    public async Task<Course> UpdateCourseAsync(Guid courseId, UpdateCourseRequest request)
    {
        var course = await _courseRepository.GetByIdAsync(courseId);
        if (course is null)
            throw new KeyNotFoundException($"Course with id {courseId} not found");

        course.Update(request.Name, request.Description, request.QueueMode);

        return await _courseRepository.UpdateAsync(course);
    }

    public async Task<Course> UpdateCoursePhotoAsync(Guid courseId, Stream stream, string fileName)
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

        course.Update(photoUri: "/" + filePath);

        return await _courseRepository.UpdateAsync(course);
    }

    public async Task DeleteCourseAsync(Guid courseId)
    {
        if (await _courseRepository.GetByIdAsync(courseId) is null)
            throw new KeyNotFoundException($"Course with '{courseId}' not found.");

        await _courseRepository.DeleteAsync(courseId);
    }
}