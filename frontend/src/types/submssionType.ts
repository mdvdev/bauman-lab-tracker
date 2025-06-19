import { Course } from "./courseType"
import { Lab } from "./labType";
import { Slot } from "./slotType"
import { User } from "./userType";
export type Submission = {
    id: string,
    student: User,
    lab: Lab,
    slot: Slot,
    teacher: User,
    course: Course,
    submissionStatus: string,
    score: number,
    comment: string,
    createdAt: string,
    updatedAt: string
};
