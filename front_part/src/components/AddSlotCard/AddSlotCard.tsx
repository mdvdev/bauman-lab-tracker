import { useState, useEffect } from "react";
import { authFetch } from "../../utils/authFetch";
import "react-datepicker/dist/react-datepicker.css";
import DatePicker from 'react-datepicker';
import "./AddSlotCard.css"
type AddSlotCardProps = {
    // onSave: () => void;
    onClose: () => void;
    courseId: string,
    teacherId: string,
}

const AddSlotCard: React.FC<AddSlotCardProps> = ({ onClose, courseId, teacherId }) => {
    const [maxStudents, setMaxStudents] = useState<string>('');
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [success, setSuccess] = useState(false);
    const [startTime, setStartTime] = useState<Date | null>(new Date());
    const [endTime, setEndTime] = useState<Date | null>(new Date());

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        setLoading(true);
        setError(null);

        try {
            const response = await authFetch(`/api/v1/courses/${courseId}/slots`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    teacherId,
                    startTime: startTime?.toISOString(),
                    endTime: endTime?.toISOString(),
                    maxStudents: Number(maxStudents),
                }),
            });

            if (!response.ok) throw new Error('Ошибка при создании курса');

            setSuccess(true);
            onClose();
        } catch (err) {
            setError((err as Error).message);
        } finally {
            setLoading(false);
        }
    };
    return (
        <form onSubmit={handleSubmit} className="add-course-form">
            <label>
                Дата начала
                <DatePicker
                    selected={startTime}
                    onChange={(date) => setStartTime(date)}
                    showTimeSelect
                    timeFormat="HH:mm"
                    timeIntervals={15}
                    dateFormat="MMMM d, yyyy h:mm aa"
                    className="date-picker-input"
                />
            </label>

            <label>
                Дата конца
                <DatePicker
                    selected={endTime}
                    onChange={(date) => setEndTime(date)}
                    showTimeSelect
                    timeFormat="HH:mm"
                    timeIntervals={15}
                    dateFormat="MMMM d, yyyy h:mm aa"
                    className="date-picker-input"
                />
            </label>

            <label>
                Максимальное количество студентов
                <input
                    type="number"
                    value={maxStudents}
                    onChange={(e) => setMaxStudents(e.target.value)}
                />
            </label>


            <button type="submit" className="create-slot-button">Создать</button>

        </form>
    );
}

export default AddSlotCard;