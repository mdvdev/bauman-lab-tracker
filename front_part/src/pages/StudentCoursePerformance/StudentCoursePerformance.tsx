import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";

import TeachersList from "../../components/TeachersList/TeachersList";
import LabStatusCard from "../../components/LabStatusCard/LabStatusCard";
import Modal from "../../components/Modal/Modal";
import LabFormCard from "../../components/LabFormCard/LabFormCard";
import { getCourseQueueMode } from "../../utils/queueModeUtil"
import { Course } from '../../types/courseType';
import { User } from "../../types/userType";
import { Lab } from "../../types/labType";
import { CourseTeacher } from "../../types/courseTeacherType";

import { PlusIcon } from "lucide-react";
import { authFetch } from "../../utils/authFetch";

import "./StudentCoursePerformance.css";

function StudentCoursePerformance() {
    const { courseId } = useParams();
    const [course, setCourse] = useState<Course>();
    const [courseTeachers, setCourseTeachers] = useState<User[]>([]);
    const [labs, setLabs] = useState<Lab[]>([]);
    const [myUserInfo, setMyUserInfo] = useState<User>();
    const [modalType, setModalType] = useState<'addLab' | 'courseParticipant' | null>(null);
    const [isModalOpen, setIsModalOpen] = useState<boolean>(false);
    const navigate = useNavigate();

    useEffect(() => {
        const fetchData = async () => {
            try {
                // 1. Курс
                const courseRes = await authFetch(`/api/v1/courses/${courseId}`);
                const courseData: Course = await courseRes.json();
                setCourse(courseData);

                // 2. Мой пользователь
                const userRes = await authFetch('/api/v1/users/me');
                const userData: User = await userRes.json();
                setMyUserInfo(userData);

                // 3. Преподаватели курса
                const teachersRes = await authFetch(`/api/v1/courses/${courseId}/teachers`);
                const teacherData: CourseTeacher[] = await teachersRes.json();
                const teachers = teacherData.map(ct => ct.user);
                setCourseTeachers(teachers);

                // 4. Лабораторные работы
                const labRes = await authFetch(`/api/v1/courses/${courseId}/labs`);
                const labData: Lab[] = await labRes.json();
                setLabs(labData);

            } catch (error) {
                console.error("Ошибка при загрузке данных:", error);
            }
        };

        fetchData();
    }, [courseId]);

    return (
        <>
            <div className="page">
                <main className="course-detail">
                    <div className="course-performance-header">
                        <h2 className="course-title">{course?.name}</h2>

                        {myUserInfo && (myUserInfo.roles.includes('Administrator') || myUserInfo.roles.includes('Teacher')) && (
                            <div className="teacher-controls-button">
                                <button
                                    className="add-lab-button"
                                    onClick={() => {
                                        setIsModalOpen(true);
                                        setModalType('addLab');
                                    }}
                                >
                                    <PlusIcon className="plus-icon" />
                                </button>
                                <button className="slots-list" onClick={() => navigate(`/courses/${courseId}`)}>
                                    Перейти к слотам
                                </button>
                            </div>
                        )}

                        <button
                            className="course-students-button"
                            onClick={() => {
                                navigate(`/courses/${courseId}/students`)
                            }}
                        >
                            Список пользователей
                        </button>
                    </div>

                    <TeachersList teachers={courseTeachers} />
                    <div className="queuemode-field">Режим работы:
                        <div className="queuemode">{getCourseQueueMode(course?.queueMode!)}</div>
                    </div>
                    <div className="lab-status-card-list">
                        {labs.map((lab) => (
                            <LabStatusCard key={lab.id} labId={lab.id} courseId={courseId || ''} />
                        ))}
                    </div>
                </main>
            </div>

            {isModalOpen && (
                <Modal onClose={() => setIsModalOpen(false)}>
                    {modalType === "addLab" && course?.id && (
                        <LabFormCard onClose={() => setIsModalOpen(false)} courseId={course.id} mode="add" labId="" />
                    )}
                    {/* {modalType === "courseParticipant" && myUserInfo?.id && (
                        <CourseParticipant
                            onClose={() => setIsModalOpen(false)}
                            courseId={course?.id!}
                            currentUserId={myUserInfo.id}
                        />
                    )} */}
                </Modal>
            )}
        </>
    );
}

export default StudentCoursePerformance;
