import { useEffect, useState } from 'react';
import './StudentCourses.css';
import { User } from '../../types/userType';
import { useNavigate } from 'react-router-dom';
import TeachersList from '../../components/TeachersList/TeachersList';
import { Course } from '../../types/courseType';
import { PlusIcon } from '@heroicons/react/24/solid';
import Modal from '../../components/Modal/Modal';
import AddCourseCard from '../../components/AddCourseCard/AddCourseCard';
import { CourseTeacher } from '../../types/courseTeacherType';
import { authFetch } from '../../utils/authFetch';

function StudentCourses() {
    const [courses, setCourses] = useState<Course[]>([]);
    const [courseTeachers, setCourseTeachers] = useState<Record<string, User[]>>({});
    const [user, setUser] = useState<User>();
    const navigate = useNavigate();
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [isAdmOrTeacher, setIsAdmOrTeacher] = useState<boolean>();
    useEffect(() => {
        const fetchUser = async () => {
            try {
                const res = await authFetch('/api/v1/users/me');
                const userData = await res.json();
                setUser(userData);
                if (userData.roles.includes('Administrator') || userData.roles.includes('Teacher')) {
                    setIsAdmOrTeacher(true);
                } else {
                    setIsAdmOrTeacher(false);
                }
            } catch (err) {
                console.error('Ошибка загрузки пользователя:', err);
            }
        };

        const fetchCoursesAndTeachers = async () => {
            try {
                const res = await authFetch('/api/v1/courses/me');
                const courseList: Course[] = await res.json();
                setCourses(courseList);

                const teachersData = await Promise.all(
                    courseList.map(course =>
                        authFetch(`/api/v1/courses/${course.id}/teachers`)
                            .then(res => res.ok ? res.json() : [])
                            .then((teachers: CourseTeacher[]) => ({
                                courseId: course.id,
                                teachers: teachers.map(t => t.user),
                            }))
                            .catch(() => ({ courseId: course.id, teachers: [] }))
                    )
                );

                const teachersMap = teachersData.reduce((acc, item) => {
                    acc[item.courseId] = item.teachers;
                    return acc;
                }, {} as Record<string, User[]>);

                setCourseTeachers(teachersMap);
            } catch (err) {
                console.error('Ошибка загрузки курсов:', err);
            }
        };

        fetchUser();
        fetchCoursesAndTeachers();
    }, []);
    return (
        <>
            <div className='page'>
                <div className='course-page-header'>
                    <h2>Ваши курсы</h2>
                    {user && (user.roles.includes('Administrator') || user.roles.includes('Teacher')) && (
                        <button
                            className="add-course-button"
                            onClick={() => setIsModalOpen(true)}
                        >
                            <PlusIcon className="plus-icon" />
                        </button>
                    )}
                </div>
                <div className='course-grid'>
                    {courses.length === 0 ? (
                        <div className="no-courses-message">
                            Вы пока не записаны ни на какой из курсов.
                        </div>
                    ) : (
                        courses.map(course => {
                            const teachers = courseTeachers[course.id] || [];
                            return (
                                <div key={course.id} className="course-card">
                                    <div className="course">
                                        <img
                                            className="course-photo"
                                            src={`http://localhost:5272${course.photoUri}`}
                                            alt={course.name}
                                        />
                                        <div className='course-info'>
                                            <span className='course-name'>{course.name}</span>
                                            <TeachersList teachers={teachers} />
                                            <div className='buttons'>
                                                <button
                                                    className='labarotory-work-record'
                                                    onClick={() => navigate(`/courses/${course.id}`)}
                                                >
                                                    {isAdmOrTeacher ? "Перейти к слотам" : "Записаться на лабораторную работу"}
                                                </button>
                                                <button
                                                    className='view-progress'
                                                    onClick={() => navigate(`/courses/${course.id}/notifications`)}
                                                >
                                                    {isAdmOrTeacher ? "Перейти к успеваемости студентов" : "Перейти к успеваемости"}
                                                </button>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            );
                        })
                    )}
                </div>
            </div>

            {isModalOpen && (
                <Modal onClose={() => setIsModalOpen(false)}>
                    <AddCourseCard onClose={() => setIsModalOpen(false)} />
                </Modal>
            )}
        </>
    );
}

export default StudentCourses;
