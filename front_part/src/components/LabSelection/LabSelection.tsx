import { useState, useEffect } from "react";
import { Lab } from "../../types/labType";
import { Submission } from "../../types/submssionType";
import "./LabSelection.css";

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

    useEffect(() => {
        const fetchData = async () => {
            try {
                const labsRes = await fetch(`http://localhost:3001/labs?courseId=${courseId}`);
                const labsData: Lab[] = await labsRes.json();
                setLabs(labsData);

                const submissionsRes = await fetch(`http://localhost:3001/submissions`);
                const submissionsData: Submission[] = await submissionsRes.json();
                setSubmissions(submissionsData);

                // Фильтруем лабораторные, по которым уже есть "Approved" или "Approved after deadline"
                const completedLabIds = submissionsData
                    .filter(sub =>
                        sub.userId === userId &&
                        (sub.status === 'Approved' || sub.status === 'Approved after deadline')
                    )
                    .map(sub => sub.labId);

                const filtered = labsData.filter(lab => !completedLabIds.includes(lab.id));
                setFilteredLabs(filtered);
            } catch (err) {
                console.error("Ошибка при загрузке данных", err);
            }
        };

        fetchData();
    }, [courseId, userId]);

    const handleSubmit = async () => {
        if (!selectedOption) return;
        setLoading(true);
        setError(null);

        try {
            const response = await fetch(`http://localhost:3001/submissions`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ slotId, selectedOption, userId }),
            });

            if (!response.ok) throw new Error('Ошибка при записи на лабораторную');

            const data = await response.json();
            setSuccess(true);
            successSign(data.id);
            onClose();
        } catch (err) {
            setError((err as Error).message);
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
