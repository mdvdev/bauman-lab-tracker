import { useEffect, useState } from 'react';
import './StudentCourses.css'
import { User } from '../../types/userType'
import { useNavigate } from 'react-router-dom';
import TeachersList from '../../components/TeachersList/TeachersList';
import { Course } from '../../types/courseType';
import { PlusIcon } from '@heroicons/react/24/solid';
import Modal from '../../components/Modal/Modal';
import AddCourseCard from '../../components/AddCourseCard/AddCourseCard';
import { CourseTeacher } from '../../types/courseTeacherType';
function StudentCourses() {
    const [courses, setCourses] = useState<Course[]>([]);
    const [courseTeachers, setCourseTeachers] = useState<Record<string, User[]>>({});
    const [user, setUser] = useState<User>();
    const navigate = useNavigate();
    const [isModalOpen, setIsModalOpen] = useState(false);

    useEffect(() => {
        // Загрузка данных пользователя
        fetch('api/v1/users/me')
            .then(res => res.json())
            .then((data: User) => setUser(data))
            .catch(err => console.error("Ошибка загрузки пользователя:", err));

        // Загрузка курсов и преподавателей
        fetch('api/v1/courses')
            .then(res => res.json())
            .then(async (courses: Course[]) => {
                setCourses(courses);

                // Загружаем преподавателей для каждого курса
                const teachersData = await Promise.all(
                    courses.map(course =>
                        fetch(`api/v1/courses/${course.id}/teachers`)
                            .then(res => res.json())
                            .then((teachers: CourseTeacher[]) => ({
                                courseId: course.id,
                                teachers: teachers.map(t => t.user)
                            }))
                            .catch(() => ({ courseId: course.id, teachers: [] }))
                    )
                );

                // Преобразуем в объект { courseId: User[] }
                const teachersMap = teachersData.reduce((acc, item) => {
                    acc[item.courseId] = item.teachers;
                    return acc;
                }, {} as Record<string, User[]>);

                setCourseTeachers(teachersMap);
            })
            .catch(err => console.error("Ошибка загрузки курсов:", err));
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
                    {courses.map(course => {
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
                                                Записаться на лабораторную работу
                                            </button>
                                            <button
                                                className='view-progress'
                                                onClick={() => navigate(`/courses/${course.id}/notifications`)}
                                            >
                                                Перейти к успеваемости
                                            </button>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        );
                    })}
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