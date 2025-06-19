import { Course } from "./courseType"
import { User } from "./userType"

export type CourseTeacher = {
    assignedAt: string,
    course: Course,
    user: User,
    score: string
}
