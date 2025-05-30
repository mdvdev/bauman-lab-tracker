import { useState, useEffect } from "react";
import { Lab } from "../../types/labType";
import { Submission } from "../../types/submssionType";
import "./LabSelection.css";
import { authFetch } from "../../utils/authFetch";

type LabSelectionProps = {
    courseId: string;
    userId: string;
    slotId: string;
    onClose: () => void;
    successSign: (submissionId: number) => void;
};

const LabSelection: React.FC<LabSelectionProps> = ({ courseId, userId, slotId, onClose, successSign }) => {
    const [labs, setLabs] = useState<Lab[]>([]);
    const [filteredLabs, setFilteredLabs] = useState<Lab[]>([]);
    const [submissions, setSubmissions] = useState<Submission[]>([]);
    const [selectedOption, setSelectedOption] = useState<string>('');
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [success, setSuccess] = useState(false);
    console.log(slotId);

    useEffect(() => {
        const fetchData = async () => {
            try {
                const labsRes = await authFetch(`/api/v1/courses/${courseId}/labs`);
                const labsData: Lab[] = await labsRes.json();
                console.log(labsData)
                setLabs(labsData);

                const submissionsRes = await authFetch(`/api/v1/courses/${courseId}/submissions`);
                const submissionsData: Submission[] = await submissionsRes.json();
                setSubmissions(submissionsData);

                // Фильтруем лабораторные, по которым уже есть "Approved" или "Approved after deadline"
                const completedLabIds = submissionsData
                    .filter(sub =>
                        sub.submissionStatus === 'Approved' ||
                        sub.submissionStatus === 'Approved after deadline'
                    )
                    .map(sub => sub.lab.id); // Теперь обращаемся к lab.id через объект lab

                const filtered = labsData.filter(lab =>
                    !completedLabIds.includes(lab.id) // Сравниваем id лабораторных работ
                );

                setFilteredLabs(filtered);
            } catch (err) {
                console.error("Ошибка при загрузке данных", err);
            }
        };

        fetchData();
    }, [courseId, userId]);

    const handleSubmit = async () => {
        if (!selectedOption) {
            setError('Выберите лабораторную работу');
            return;
        }

        setLoading(true);
        setError(null);

        try {
            const requestBody = {
                studentId: userId,
                labId: selectedOption,
                slotId: slotId
            };
            const response = await fetch(`/api/v1/courses/${courseId}/submissions`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(requestBody),
            });

            if (!response.ok) {
                const errorData = await response.json().catch(() => ({}));
                throw new Error(errorData.message || 'Ошибка при записи на лабораторную');
            }

            const data = await response.json();
            setSuccess(true);
            successSign(data.id);
            onClose();
        } catch (err) {
            console.error('Submission error:', err);
            setError((err as Error).message || 'Произошла ошибка');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="lab-selection-modal">
            <label htmlFor="lab-select" className="lab-label">
                Выберите Лабораторную работу
            </label>
            <select
                id="lab-select"
                className="lab-select"
                value={selectedOption}
                onChange={(e) => setSelectedOption(e.target.value)}
            >
                {filteredLabs.map((lab) => (
                    <option key={lab.id} value={lab.id}>
                        {lab.name}
                    </option>
                ))}
            </select>
            <button onClick={handleSubmit} disabled={loading || !selectedOption}>
                {loading ? 'Запись...' : success ? 'Записано' : 'Записаться'}
            </button>
            {error && <p className="error-text">{error}</p>}
        </div>
    );
};

export default LabSelection;
