import { useEffect, useState } from "react";
import "./SignUpForLaboratoryWorkPage.css";
import { useParams } from "react-router-dom";

import TeachersList from "../../components/TeachersList/TeachersList";
import SlotCard from '../../components/SlotCard/SlotCard';

import { User } from "../../types/userType";
import { Slot } from "../../types/slotType";
import { Course } from "../../types/courseType";
import { CourseTeacher } from "../../types/courseTeacherType";

import { authFetch } from "../../utils/authFetch";

function SignUpForLaboratoryWorkPage() {
    const { courseId } = useParams();
    const [courseTeachers, setCourseTeachers] = useState<User[]>([]);
    const [course, setCourse] = useState<Course | null>(null);
    const [courseSlots, setCourseSlots] = useState<Slot[]>([]);
    const [user, setUser] = useState<User | null>(null);
    const [isAdmOrTeacher, setIsAdmOrTeacher] = useState<boolean>();
    useEffect(() => {
        const fetchData = async () => {
            try {
                const userRes = await authFetch(`/api/v1/users/me`);
                const userData: User = await userRes.json();
                setUser(userData);
                if (userData.roles.includes('Administrator') || userData.roles.includes('Teacher')) {
                    setIsAdmOrTeacher(true);
                } else {
                    setIsAdmOrTeacher(false);
                }
                const courseRes = await authFetch(`/api/v1/courses/${courseId}`);
                const courseData: Course = await courseRes.json();
                setCourse(courseData);

                const slotsRes = await authFetch(`/api/v1/courses/${courseId}/slots`);
                const slotsData: Slot[] = await slotsRes.json();
                setCourseSlots(slotsData);

                const teachersRes = await authFetch(`/api/v1/courses/${courseId}/teachers`);
                const teachersData: CourseTeacher[] = await teachersRes.json();
                const teachers = teachersData.map(ct => ct.user);
                setCourseTeachers(teachers);

            } catch (error) {
                console.error("Ошибка при загрузке данных:", error);
            }
        };

        fetchData();
    }, [courseId]);

    return (
        <div className="page">
            <main className="course-detail">
                <h2 className="course-title">{course?.name}</h2>

                <TeachersList teachers={courseTeachers} />

                <h3>{isAdmOrTeacher ? "Слоты:" : "Слоты для записи"}</h3>
                <div className="course-slots">
                    {courseSlots.map((slot) => (
                        <SlotCard
                            key={slot.id}
                            slot={slot}
                            courseId={courseId!}
                            userId={user?.id!}
                        />
                    ))}
                </div>
            </main>
        </div>
    );
}

export default SignUpForLaboratoryWorkPage;
