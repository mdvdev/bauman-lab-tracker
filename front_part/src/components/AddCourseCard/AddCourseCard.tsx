import { useState, useEffect } from "react";
import "./AddCourseCard.css"
type AddCourseCardProps = {
    // onSave: () => void;
    onClose: () => void;
}

const AddCourseCard: React.FC<AddCourseCardProps> = ({ onClose }) => {
    const [selectedOption, setSelectedOption] = useState<string>('');
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [success, setSuccess] = useState(false);
    const [courseName, setCourseName] = useState<string>('');
    const [courseDescription, setCourseDescription] = useState<string>('');
    const handleChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
        setSelectedOption(e.target.value);
    };
    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        setLoading(true);
        setError(null);

        try {
            const response = await fetch(`/api/v1/courses`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    name: courseName,
                    description: courseDescription,
                    queueMode: selectedOption,
                }),
            });

            if (!response.ok) throw new Error('Ошибка при записи на лабораторную');

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
                Название курса
                <input
                    type="text"
                    value={courseName}
                    onChange={(e) => setCourseName(e.target.value)}
                />
            </label>

            <label>
                Описание курса
                <input
                    type="text"
                    value={courseDescription}
                    onChange={(e) => setCourseDescription(e.target.value)}
                />
            </label>

            <label>
                Выберите режим работы:
                <select value={selectedOption} onChange={handleChange}>
                    <option value="Democratic">Демократический</option>
                    <option value="Oligarchic">Олигархический</option>
                    <option value="Anarchic">Анархический</option>
                </select>
            </label>


            <button type="submit" className="create-course-button">Создать</button>

        </form>
    );
}

export default AddCourseCard;