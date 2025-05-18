import { useEffect, useState } from "react";
import TeachersList from "../../components/TeachersList/TeachersList";
import { useParams } from "react-router-dom";
import { Course } from '../../types/courseType'
import { User } from "../../types/userType";
import { Lab } from "../../types/labType";
import LabStatusCard from "../../components/LabStatusCard/LabStatusCard"
import { Submission } from "../../types/submssionType"

import "./StudentCoursePerformance.css"
function StudentCoursePerformance() {
    const { courseId } = useParams();
    const [course, setCourse] = useState<Course>();
    const [courseTeachers, setCourseTeachers] = useState<User[]>([]);
    // const [notifications, setNotifications] = useState<Notification | null>(null);
    const [labs, setLabs] = useState<Lab[]>([]);
    const [submissions, setSubmissions] = useState<Submission[]>([]);

    useEffect(() => {
        const fetchData = async () => {
            try {
                const courseRes = await fetch(`http://localhost:3001/courses/${courseId}`);
                const courseData: Course = await courseRes.json();
                setCourse(courseData);

                const usersRes = await fetch(`http://localhost:3001/users`);
                const allUsers: User[] = await usersRes.json();

                const filteredTeachers = allUsers.filter(user =>
                    courseData.teacherIds.includes(user.id));
                setCourseTeachers(filteredTeachers);

                const labRes = await fetch(`http://localhost:3001/labs`);
                const labData: Lab[] = await labRes.json();
                const filteredLabs = labData.filter(lab => lab.courseId === courseId);
                setLabs(filteredLabs);

                const submissionsRes = await fetch(`http://localhost:3001/submissions`);
                const submissionsData: Submission[] = await submissionsRes.json();
                setSubmissions(submissionsData);

            } catch (error) {
                console.error("Error fetching data:", error);
            }
        };
        fetchData();

    }, [courseId])

    return (
        <div className="page">
            <main className="course-detail">
                <h2 className="course-title">{course?.name}</h2>
                <TeachersList teachers={courseTeachers} />
                <div className="lab-status-card-list">
                    {labs.map((lab) => {
                        const matchedSubmission = submissions.find(sub => sub.labId === lab.id) || null;
                        return (
                            <LabStatusCard
                                key={lab.id}
                                labId={lab.id}
                                submission={matchedSubmission}
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
