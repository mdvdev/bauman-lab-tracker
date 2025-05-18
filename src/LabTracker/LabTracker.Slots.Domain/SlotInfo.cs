using LabTracker.Courses.Domain;
using LabTracker.Users.Domain;

namespace LabTracker.Slots.Domain;

public record SlotInfo(Slot Slot, Course Course, User Teacher);