import { useState, useEffect } from "react";
import "react-datepicker/dist/react-datepicker.css";
import DatePicker from "react-datepicker";
import { authFetch } from "../../utils/authFetch";
import "./LabFormCard.css";

type LabFormCardProps = {
    onClose: () => void;
    courseId: string;
    mode: "add" | "edit";
    labId?: string;
};

const LabFormCard: React.FC<LabFormCardProps> = ({
    onClose,
    courseId,
    mode,
    labId,
}) => {
    const [labScore, setLabScore] = useState<string>("");
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [success, setSuccess] = useState(false);
    const [labName, setLabName] = useState<string>("");
    const [labScoreAfterDeadline, setLabScoreAfterDeadline] = useState<string>("");
    const [selectedDate, setSelectedDate] = useState<Date | null>(new Date());
    const [descriptionFile, setDescriptionFile] = useState<File | null>(null);

    const uploadLabDescription = async (courseId: string, labId: string, file: File) => {
        const formData = new FormData();
        formData.append("file", file);

        const res = await authFetch(`/api/v1/courses/${courseId}/labs/${labId}/description`, {
            method: "PATCH",
            body: formData,
        });

        if (!res.ok) {
            const text = await res.text();
            console.error("Ошибка загрузки файла описания:", text);
            throw new Error("Ошибка загрузки файла описания");
        }
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setLoading(true);
        setError(null);

        try {
            let labIdFinal = labId;

            if (mode === "add") {
                const response = await authFetch(`/api/v1/courses/${courseId}/labs`, {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify({
                        name: labName,
                        deadline: selectedDate?.toISOString(),
                        score: labScore,
                        scoreAfterDeadline: labScoreAfterDeadline,
                    }),
                });

                if (!response.ok) throw new Error("Ошибка при создании лабораторной работы");

                const createdLab = await response.json();
                labIdFinal = createdLab.id;
            }

            if (mode === "edit" && labId) {
                const response = await authFetch(`/api/v1/courses/${courseId}/labs/${labId}`, {
                    method: "PATCH",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify({
                        name: labName,
                        deadline: selectedDate?.toISOString(),
                        score: labScore,
                        scoreAfterDeadline: labScoreAfterDeadline,
                    }),
                });

                if (!response.ok) throw new Error("Ошибка при обновлении лабораторной работы");
            }

            // Загружаем файл только если он выбран и labIdFinal определён
            if (descriptionFile && labIdFinal) {
                await uploadLabDescription(courseId, labIdFinal, descriptionFile);
            }

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
                <span>{mode === "add" ? "Создание лабораторной работы" : "Редактирование"}</span>
            </div>

            <label>
                Название лабораторной работы
                <input
                    type="text"
                    value={labName}
                    onChange={(e) => setLabName(e.target.value)}
                    required
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
                Максимальное количество баллов
                <input
                    type="text"
                    value={labScore}
                    onChange={(e) => setLabScore(e.target.value)}
                    required
                />
            </label>

            <label>
                Баллы за сдачу после дедлайна
                <input
                    type="text"
                    value={labScoreAfterDeadline}
                    onChange={(e) => setLabScoreAfterDeadline(e.target.value)}
                    required
                />
            </label>

            <label>
                Описание (файл)
                <input
                    type="file"
                    accept=".txt,.pdf,.doc,.docx"
                    onChange={(e) => setDescriptionFile(e.target.files?.[0] || null)}
                />
            </label>

            <button type="submit" className="create-lab-button" disabled={loading}>
                {mode === "add" ? "Создать" : "Обновить"}
            </button>

            {error && <p className="error-message">{error}</p>}
        </form>
    );
};

export default LabFormCard;
