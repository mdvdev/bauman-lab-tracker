import { useState, useEffect } from "react";
import "react-datepicker/dist/react-datepicker.css";
import DatePicker from 'react-datepicker';
import { authFetch } from "../../utils/authFetch";
import "./LabFormCard.css"
type AddFormCardProps = {
    // onSave: () => void;
    onClose: () => void;
    courseId: string;
    mode: "add" | "edit";
}

const AddFormCard: React.FC<AddFormCardProps> = ({ onClose, courseId, mode }) => {
    const [labScore, setLabScore] = useState<string>('');
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [success, setSuccess] = useState(false);
    const [labName, setLabName] = useState<string>('');
    const [labScoreAfterDeadline, setLabScoreAfterDeadline] = useState<string>('');
    const [selectedDate, setSelectedDate] = useState<Date | null>(new Date());

    const handleSubmit = async (e: React.FormEvent) => {
        const isoDate = selectedDate?.toISOString();
        console.log(isoDate);
        e.preventDefault();

        setLoading(true);
        setError(null);

        try {
            const response = await authFetch(`/api/v1/courses/${courseId}/labs`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    name: labName,
                    deadline: selectedDate?.toISOString(),
                    score: labScore,
                    scoreAfterDeadline: labScoreAfterDeadline
                }),
            });

            if (!response.ok) throw new Error('Ошибка при создании  лабораторной работы');

            setSuccess(true);
            onClose();
        } catch (err) {
            setError((err as Error).message);
        } finally {
            setLoading(false);
        }
    };
    return (
        <form onSubmit={handleSubmit} className="add-lab-form">
            <div className="student-list-header">
                <span>{mode === "add" ? "Cоздание лабороторной работы" : "Редактирование"}</span>
            </div>
            <label>
                Название лабораторной работы
                <input
                    type="text"
                    value={labName}
                    onChange={(e) => setLabName(e.target.value)}
                />
            </label>

            <label>
                Срок сдачи
                <DatePicker
                    selected={selectedDate}
                    onChange={(date) => setSelectedDate(date)}
                    showTimeSelect
                    timeFormat="HH:mm"
                    timeIntervals={15}
                    dateFormat="MMMM d, yyyy h:mm aa"
                    className="date-picker-input"
                />
            </label>

            <label>
                Максимальное кол-во баллов
                <input
                    type="text"
                    value={labScore}
                    onChange={(e) => setLabScore(e.target.value)}
                />
            </label>

            <label>
                Кол-во баллов не в срок
                <input
                    type="text"
                    value={labScoreAfterDeadline}
                    onChange={(e) => setLabScoreAfterDeadline(e.target.value)}
                />
            </label>


            <button type="submit" className="create-lab-button">{mode === "add" ? "Создать" : "Обновить"}</button>

        </form>
    );
}

export default AddFormCard;