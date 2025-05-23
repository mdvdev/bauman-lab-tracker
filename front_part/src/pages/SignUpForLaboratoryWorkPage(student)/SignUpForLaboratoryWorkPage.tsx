import { useEffect, useState } from "react";
import "./SignUpForLaboratoryWorkPage.css"
import { useParams } from "react-router-dom";
import TeachersList from "../../components/TeachersList/TeachersList"
import { User } from "../../types/userType"
import { Slot } from "../../types/slotType"
import SlotCard from '../../components/SlotCard/SlotCard'
import { Course } from "../../types/courseType";
import { CourseTeacher } from "../../types/courseTeacherType";

function SignUpForLaboratoryWorkPage() {
    const [courseTeachers, setCourseTeachers] = useState<User[]>([]);
    const [course, setCourse] = useState<Course | null>(null);
    const { courseId } = useParams();
    const [courseSlots, setCourseSlots] = useState<Slot[]>([]);
    const [user, setUser] = useState<User | null>(null);
    useEffect(() => {

        const fetchData = async () => {
            try {
                const userRes = await fetch(`/api/v1/users/me`);
                const userData: User = await userRes.json();

                setUser(userData);

                // Загрузка данных курса
                const courseRes = await fetch(`/api/v1/courses/${courseId}`);
                const courseData: Course = await courseRes.json();

                setCourse(courseData);

                // Загрузка слотов курса
                const courseSlotsRes = await fetch(`/api/v1/courses/${courseId}/slots`);
                const courseSlotsData: Slot[] = await courseSlotsRes.json();
                setCourseSlots(courseSlotsData);

                // Загрузка преподавателей курса
                const teachersRes = await fetch(`/api/v1/courses/${courseId}/teachers`);
                const teachersData: CourseTeacher[] = await teachersRes.json();

                // Преобразуем CourseTeacher[] в User[]
                const teachers = teachersData.map(ct => ct.user);
                setCourseTeachers(teachers);

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
                        <SlotCard key={slot.id} slot={slot} courseId={courseId!} userId={user?.id!} />
                    ))}
                </div>
            </main>
        </div>
    );
}

export default SignUpForLaboratoryWorkPage;