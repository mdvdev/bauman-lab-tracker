import { useEffect, useState } from "react";
import "./SignUpForLaboratoryWorkPage.css"
import { useParams } from "react-router-dom";
import TeachersList from "../../components/TeachersList/TeachersList"
import { User } from "../../types/userType"
import { Slot } from "../../types/slotType"
import SlotCard from '../../components/SlotCard/SlotCard'
interface Course {
    id: string | number;
    name: string;
    description?: string;
    photo: string;
    teacherIds: (string | number)[];
    createdAt: string;
}

function SignUpForLaboratoryWorkPage() {
    const [courseTeachers, setCourseTeachers] = useState<User[]>([]);
    const [course, setCourse] = useState<Course | null>(null);
    const { courseId } = useParams();
    const [courseSlots, setCourseSlots] = useState<Slot[]>([])
    useEffect(() => {
        const fetchData = async () => {
            try {
                const courseRes = await fetch(`http://localhost:3001/courses/${courseId}`);
                const courseData: Course = await courseRes.json();
                setCourse(courseData);

                const courseSlotsRes = await fetch(`http://localhost:3001/slots?courseId=${courseId}`);
                const courseSlotsData: Slot[] = await courseSlotsRes.json();
                setCourseSlots(courseSlotsData);

                const usersRes = await fetch(`http://localhost:3001/users`);
                const allUsers: User[] = await usersRes.json();

                const filteredTeachers = allUsers.filter(user =>
                    courseData.teacherIds.includes(user.id)
                );
                console.log("Filtered Teachers:", filteredTeachers);

                setCourseTeachers(filteredTeachers);
            } catch (error) {
                console.error("Error fetching data:", error);
            }
        };

        fetchData();
    }, [courseId]);

    return (
        <div className="page">
            <main className="course-detail">
                <h2 className="course-title">{course?.name}</h2>
                <TeachersList teachers={courseTeachers} />
                <h3>Слоты для записи:</h3>
                <div className="course-slots">
                    {courseSlots.map((slot) => (
                        <SlotCard key={slot.id} slot={slot} />
                    ))}
                </div>

            </main>
        </div>
    );
}

export default SignUpForLaboratoryWorkPage; 