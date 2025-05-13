using LabTracker.Application.Courses.Core;
using LabTracker.Application.Courses.Students;
using LabTracker.Domain.Entities;
using LabTracker.Domain.ValueObjects;

namespace LabTracker.Presentation.Utils;

public static class CourseUtils
{
    public static async Task<(Course Course, User Teacher)?> GetCourseAndTeacherAsync(
        Guid courseId,
        Guid teacherId,
        ICourseService courseService,
        ICourseMemberService courseMemberService)
    {
        var course = await courseService.GetCourseDetailsAsync(courseId);
        if (course is null) return null;

        var teacher = await courseMemberService
            .GetCourseMemberDetailsAsync(new CourseMemberKey(courseId, teacherId));
        if (teacher is null) return null;

        return (course, teacher);
    }
}