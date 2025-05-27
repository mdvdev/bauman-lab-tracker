import { useEffect, useState, useContext } from "react";
import { useNavigate, useParams } from "react-router-dom";

import TeachersList from "../../components/TeachersList/TeachersList";
import LabStatusCard from "../../components/LabStatusCard/LabStatusCard";
import Modal from "../../components/Modal/Modal";
import AddLabCard from "../../components/AddLabCard/AddLabCard";
import CourseParticipant from "../../components/CourseParticipantCard/CourseParticipantCard";

import { Course } from '../../types/courseType';
import { User } from "../../types/userType";
import { Lab } from "../../types/labType";
import { CourseTeacher } from "../../types/courseTeacherType";

import { AuthContext } from '../../AuthContext';  // путь поправь, если нужно

import { PlusIcon, Settings } from "lucide-react";

import "./StudentCoursePerformance.css";

function StudentCoursePerformance() {
    const { courseId } = useParams();
    const { credentials } = useContext(AuthContext);
    const [course, setCourse] = useState<Course>();
    const [courseTeachers, setCourseTeachers] = useState<User[]>([]);
    const [labs, setLabs] = useState<Lab[]>([]);
    const [myUserInfo, setMyUserInfo] = useState<User>();
    const [modalType, setModalType] = useState<'addLab' | 'courseParticipant' | null>(null);
    const [isModalOpen, setIsModalOpen] = useState<boolean>(false);
    const navigate = useNavigate();

    useEffect(() => {
        const fetchData = async () => {
            if (!credentials) {
                console.warn("User is not authenticated yet");
                return;
            }

            const authHeader = 'Basic ' + btoa(`${credentials.email}:${credentials.password}`);

            try {
                // 1. Получаем данные курса
                const courseRes = await fetch(`/api/v1/courses/${courseId}`, {
                    headers: {
                        'Authorization': authHeader,
                        'Content-Type': 'application/json',
                    },
                });
                if (!courseRes.ok) throw new Error('Failed to fetch course');
                const courseData: Course = await courseRes.json();
                setCourse(courseData);

                // 2. Получаем информацию о текущем пользователе
                const myUserInfoRes = await fetch(`/api/v1/users/me`, {
                    headers: {
                        'Authorization': authHeader,
                        'Content-Type': 'application/json',
                    },
                });
                if (!myUserInfoRes.ok) throw new Error('Failed to fetch user info');
                const myUserInfoData: User = await myUserInfoRes.json();
                setMyUserInfo(myUserInfoData);

                // 3. Получаем список преподавателей курса
                const teachersRes = await fetch(`/api/v1/courses/${courseId}/teachers`, {
                    headers: {
                        'Authorization': authHeader,
                        'Content-Type': 'application/json',
                    },
                });
                if (!teachersRes.ok) throw new Error('Failed to fetch teachers');
                const teachersData: CourseTeacher[] = await teachersRes.json();
                const teachers = teachersData.map(ct => ct.user);
                setCourseTeachers(teachers);

                // 4. Получаем список лабораторных работ
                const labRes = await fetch(`/api/v1/courses/${courseId}/labs`, {
                    headers: {
                        'Authorization': authHeader,
                        'Content-Type': 'application/json',
                    },
                });
                if (!labRes.ok) throw new Error('Failed to fetch labs');
                const labData: Lab[] = await labRes.json();
                setLabs(labData);

            } catch (error) {
                console.error("Error fetching data:", error);
            }
        };

        fetchData();

    }, [courseId, credentials]);  // обновляем при изменении courseId или credentials

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
                                <button className="admin-button">
                                    <Settings stroke="white" className="settings-icon" />
                                </button>
                                <button className="slots-list" onClick={() => navigate(`/courses/${courseId}`)}>
                                    Перейти к слотам
                                </button>
                            </div>
                        )}

                        <button
                            className="course-students-button"
                            onClick={() => {
                                setIsModalOpen(true);
                                setModalType('courseParticipant');
                            }}
                        >
                            Список пользователей
                        </button>
                    </div>

                    <TeachersList teachers={courseTeachers} />

                    <div className="lab-status-card-list">
                        {labs.map((lab) => (
                            <LabStatusCard key={lab.id} labId={lab.id} courseId={courseId || ''} />
                        ))}
                    </div>
                </main>
            </div>

            {isModalOpen && (
                <Modal onClose={() => setIsModalOpen(false)}>
                    {modalType === "addLab" && (
                        <AddLabCard onClose={() => setIsModalOpen(false)} courseId={course?.id!} />
                    )}
                    {modalType === "courseParticipant" && (
                        <CourseParticipant
                            onClose={() => setIsModalOpen(false)}
                            courseId={course?.id!}
                            currentUserId={myUserInfo?.id!}
                        />
                    )}
                </Modal>
            )}
        </>
    );
}

export default StudentCoursePerformance;
