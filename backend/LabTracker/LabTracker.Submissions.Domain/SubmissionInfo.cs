using LabTracker.Courses.Domain;
using LabTracker.Labs.Domain;
using LabTracker.Slots.Domain;
using LabTracker.Users.Domain;

namespace LabTracker.Submissions.Domain;

public record SubmissionInfo(
    User Student,
    Lab Lab,
    SlotInfo SlotInfo,
    Course Course,
    Submission Submission
);