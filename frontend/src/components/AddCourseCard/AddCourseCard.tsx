import { useEffect, useState } from "react";
import "./AddCourseCard.css";
import { authFetch } from "../../utils/authFetch";

type AddCourseCardProps = {
    onClose: () => void;
    mode: 'updateCourse' | 'addCourse';
    courseId: string;
};

const AddCourseCard: React.FC<AddCourseCardProps> = ({ onClose, mode, courseId }) => {
    const [selectedOption, setSelectedOption] = useState<string>('Democratic');
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [success, setSuccess] = useState(false);
    const [courseName, setCourseName] = useState<string>('');
    const [courseDescription, setCourseDescription] = useState<string>('');

    useEffect(() => {
        if (mode === 'updateCourse' && courseId) {
            const fetchCourse = async () => {
                try {
                    const res = await authFetch(`/api/v1/courses/${courseId}`);
                    if (!res.ok) throw new Error('Ошибка при загрузке курса');
                    const data = await res.json();
                    setCourseName(data.name);
                    setCourseDescription(data.description);
                    setSelectedOption(data.queueMode || 'Democratic');
                } catch (err) {
                    console.error('Ошибка при загрузке курса:', err);
                    setError('Не удалось загрузить данные курса.');
                }
            };

            fetchCourse();
        }
    }, [mode, courseId]);

    const handleChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
        setSelectedOption(e.target.value);
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setLoading(true);
        setError(null);

        try {
            const method = mode === 'addCourse' ? 'POST' : 'PATCH';
            const path = mode === 'addCourse'
                ? '/api/v1/courses'
                : `/api/v1/courses/${courseId}`;

            const response = await authFetch(path, {
                method,
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    name: courseName,
                    description: courseDescription,
                    queueMode: selectedOption,
                }),
            });

            if (!response.ok) throw new Error('Ошибка при сохранении курса');
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
                    required
                />
            </label>

            <label>
                Описание курса
                <input
                    type="text"
                    value={courseDescription}
                    onChange={(e) => setCourseDescription(e.target.value)}
                    required
                />
            </label>

            <label>
                Режим работы:
                <select value={selectedOption} onChange={handleChange}>
                    <option value="Democratic">Демократический</option>
                    <option value="Oligarchic">Олигархический</option>
                    <option value="Anarchic">Анархический</option>
                </select>
            </label>

            {error && <div className="error-message">{error}</div>}
            {success && <div className="success-message">Курс успешно сохранён</div>}

            <button type="submit" className="create-course-button" disabled={loading}>
                {loading ? "Сохранение..." : mode === 'addCourse' ? "Создать курс" : "Обновить курс"}
            </button>
        </form>
    );
};

export default AddCourseCard;
