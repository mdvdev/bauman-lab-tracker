import { useEffect, useState } from "react";
import TeachersList from "../../components/TeachersList/TeachersList";
import { useNavigate, useParams } from "react-router-dom";
import { Course } from '../../types/courseType'
import { User } from "../../types/userType";
import { Lab } from "../../types/labType";
import LabStatusCard from "../../components/LabStatusCard/LabStatusCard"
import { Submission } from "../../types/submssionType"
import { PlusIcon } from "lucide-react";
import { Settings } from "lucide-react";
import { CourseTeacher } from "../../types/courseTeacherType";
import "./StudentCoursePerformance.css"
function StudentCoursePerformance() {
    const { courseId } = useParams();
    const [course, setCourse] = useState<Course>();
    const [courseTeachers, setCourseTeachers] = useState<User[]>([]);
    // const [notifications, setNotifications] = useState<Notification | null>(null);
    const [labs, setLabs] = useState<Lab[]>([]);
    const [myUserInfo, setMyUserInfo] = useState<User>();
    const navigate = useNavigate();

    useEffect(() => {
        const fetchData = async () => {
            try {
                const courseRes = await fetch(`/api/v1/courses/${courseId}`);
                const courseData: Course = await courseRes.json();
                setCourse(courseData);

                const myUserInfoRes = await fetch(`/api/v1/users/me`);
                const myUserInfoData: User = await myUserInfoRes.json();
                setMyUserInfo(myUserInfoData);

                const teachersRes = await fetch(`/api/v1/courses/${courseId}/teachers`);
                const teachersData: CourseTeacher[] = await teachersRes.json();

                const teachers = teachersData.map(ct => ct.user);
                setCourseTeachers(teachers);

                const labRes = await fetch(`/api/v1/courses/${courseId}/labs`);
                const labData: Lab[] = await labRes.json();
                console.log(labData);
                setLabs(labData);

            } catch (error) {
                console.error("Error fetching data:", error);
            }
        };
        fetchData();

    }, [courseId])

    return (
        <div className="page">
            <main className="course-detail">
                <div className="course-performance-header">
                    <h2 className="course-title">{course?.name}</h2>
                    {myUserInfo && (myUserInfo.roles.includes('Administrator') || myUserInfo.roles.includes('Teacher')) &&
                        (
                            <div className="teacher-controls-button">
                                <button className="add-lab-button"><PlusIcon className="plus-icon" /></button>
                                <button className="admin-button"><Settings stroke="white" className="settings-icon" /></button>
                                <button className="slots-list" onClick={() => navigate(`/courses/${courseId}`)}>Перейти к слотам</button>

                            </div>)

                    }
                    <button className="course-students-button">Список пользователей</button>
                </div>
                <TeachersList teachers={courseTeachers} />
                <div className="lab-status-card-list">
                    {labs.map((lab) => {
                        return (
                            <LabStatusCard
                                key={lab.id}
                                labId={lab.id}
                                courseId={courseId || ''}
                            />
                        );
                    })}
                </div>
            </main>
        </div>
    )
}
export default StudentCoursePerformance;
