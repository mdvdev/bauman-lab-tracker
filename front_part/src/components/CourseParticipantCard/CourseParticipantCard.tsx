import { useEffect, useState } from 'react';
import { CourseStudent } from '../../types/courseStudentType';
import { User } from '../../types/userType';
import './CourseParticipantCard.css';
import { authFetch } from '../../utils/authFetch';
type CourseParticipantProps = {
    courseId: string;
    currentUserId: string; // передавай ID текущего пользователя
    onClose: () => void;
};

const CourseParticipant: React.FC<CourseParticipantProps> = ({ courseId, currentUserId, onClose }) => {
    const [students, setStudents] = useState<CourseStudent[]>([]);
    const [loading, setLoading] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);

    const fetchStudents = async () => {
        try {
            setLoading(true);
            setError(null);

            const response = await authFetch(`/api/v1/courses/${courseId}/students`);
            if (!response.ok) {
                throw new Error(`Ошибка HTTP: ${response.status}`);
            }

            const data = await response.json();
            setStudents(data);
        } catch (err) {
            setError(err instanceof Error ? err.message : 'Неизвестная ошибка');
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchStudents();
    }, []);

    const formatFullName = (user: User) =>
        `${user.lastName} ${user.firstName} ${user.patronymic}`;

    return (
        <div className="student-list-container">
            <div className="student-list-header">
                <span>Список студентов</span>
            </div>
            {loading && <div>Загрузка...</div>}
            {error && <div className="error">Ошибка: {error}</div>}
            {!loading && !error && (
                <ol className="student-list">
                    {students.map((student) => {
                        const isCurrentUser = student.user.id === currentUserId;
                        return (
                            <li key={student.user.id} className="student-item">
                                {formatFullName(student.user)} {isCurrentUser && '(Вы)'}
                            </li>
                        );
                    })}
                </ol>
            )}
        </div>
    );
};

export default CourseParticipant;
