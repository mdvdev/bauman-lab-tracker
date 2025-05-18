import './TeachersList.css'
import { User } from '../../types/userType';

interface TeachersListProps {
    teachers: User[];
}

function TeachersList({ teachers }: TeachersListProps) {
    if (teachers.length === 0) return null;

    return (
        <div className='list-of-teachers'>
            <span style={{ color: 'grey' }}>{teachers.length === 1 ? "Преподаватель:" : "Преподаватели:"}</span>
            {teachers.map((teacher, index) => (
                <span key={teacher.id} className="teacher-name">
                    {teacher.firstName} {teacher.lastName}
                    {index < teachers.length - 1 && ','}
                </span>
            ))}
        </div>
    );
}

export default TeachersList;
