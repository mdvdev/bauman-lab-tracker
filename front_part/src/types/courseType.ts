export type Course = {
    id: string | number;
    name: string;
    description?: string;
    photo: string
    teacherIds: (string | number)[];
}