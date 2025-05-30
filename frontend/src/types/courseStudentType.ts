import { Course } from "./courseType"
import { User } from "./userType"

export type CourseStudent = {
    assignedAt: string,
    course: Course,
    user: User,
    createdAt: string,
    photoUri: string

}
