using Courses.Web.Dtos;
using Labs.Web.Dtos;
using LabTracker.Submissions.Domain;
using Slots.Web.Dtos;
using Users.Web.Dtos;

namespace LabTracker.Submissions.Web.Dtos;

public record SubmissionResponse(
    Guid Id,
    UserResponse Student,
    LabResponse Lab,
    SlotResponse Slot,
    CourseResponse Course,
    SubmissionStatus SubmissionStatus,
    string? Comment,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt)
{
    public static SubmissionResponse Create(SubmissionInfo submissionInfo) =>
        new(
            Id: submissionInfo.Submission.Id,
            Student: UserResponse.Create(submissionInfo.Student),
            Lab: LabResponse.Create(submissionInfo.Lab),
            Slot: SlotResponse.Create(submissionInfo.SlotInfo),
            Course: CourseResponse.Create(submissionInfo.Course),
            SubmissionStatus: submissionInfo.Submission.SubmissionStatus,
            Comment: submissionInfo.Submission.Comment,
            CreatedAt: submissionInfo.Submission.CreatedAt,
            UpdatedAt: submissionInfo.Submission.UpdatedAt
        );
}