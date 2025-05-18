import { useEffect, useState } from 'react';
import './StudentCourses.css'
import { User } from '../../types/userType'
import { useNavigate } from 'react-router-dom';
import TeachersList from '../../components/TeachersList/TeachersList';
import { Course } from '../../types/courseType';
import { PlusIcon } from '@heroicons/react/24/solid';
import Modal from '../../components/Modal/Modal';
import AddCourseCard from '../../components/AddCourseCard/AddCourseCard';
function StudentCourses() {
    const [courses, setCourses] = useState<Course[]>([]);
    const [teachers, setTeachers] = useState<User[]>([]);
    const [user, setUser] = useState<User>();
    const navigate = useNavigate();
    const [isModalOpen, setIsModalOpen] = useState(false);

    useEffect(() => {
        fetch(`http://localhost:3001/courses`)
            .then(res => res.json())
            .then((courses: Course[]) => {
                setCourses(courses);
                const teacherIds = Array.from(new Set(
                    courses.flatMap(course => course.teacherIds)
                ));
                return fetch(`http://localhost:3001/users?id=${teacherIds.join("&id=")}`);
            })
            .then(res => res.json())
            .then((teachers: User[]) => {
                setTeachers(teachers);
            })
            .catch(err => console.error("Ошибка:", err));
        fetch(`http://localhost:3001/users/1`)
            .then(res => res.json())
            .then((data: User) => setUser(data))
    }, []);
    return (
        <>
            <div className='page'>
                <div className='course-page-header'>
                    <h2>Ваши курсы</h2>
                    {user && (user.role === 'admin' || user.role === 'teacher') && (
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
                        const courseTeachers = teachers.filter(teacher => course.teacherIds.includes(teacher.id));
                        return (
                            <div key={course.id} className="course-card">
                                <div className="course">
                                    <img className="course-photo" src={course.photo} alt={course.name} />
                                    <div className='course-info'>
                                        <span className='course-name'>{course.name}</span>
                                        <TeachersList teachers={courseTeachers} />
                                        <div className='buttons'>
                                            <button className='labarotory-work-record' onClick={() => navigate(`/courses/${course.id}`)}>Записаться на лабараторную работу</button>
                                            <button className='view-progress' onClick={() => navigate(`/courses/${course.id}/notifications`)}>Перейти к успеваемости</button>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        );
                    })}
                </div>
            </div>
            {isModalOpen && <Modal onClose={() => setIsModalOpen(false)}>
                <AddCourseCard onClose={() => setIsModalOpen(false)}></AddCourseCard>
            </Modal>
            }
        </>
    );
}

export default StudentCourses;
