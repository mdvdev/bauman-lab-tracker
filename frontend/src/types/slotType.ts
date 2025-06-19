import { Course } from "./courseType"
import { User } from "./userType";
export type Slot = {
    id: string,
    course: Course,
    teacher: User;
    startTime: string,
    endTime: string,
    maxStudents: string;
}

