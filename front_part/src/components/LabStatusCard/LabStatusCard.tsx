import { useEffect, useState } from "react";
import { Lab } from "../../types/labType";
import './LabStatusCard.css'
import { getSubmissionStatusClass } from "../../utils/labStatusUtil"
import { Submission } from "../../types/submssionType"
import { LabStatus } from "../../types/labStatusType";
import { useNavigate } from 'react-router-dom';
interface LabStatusCardProps {
    labId: string;
    submission: Submission | null
    courseId: string | null;
}


function formatIsoString(isoString: string): string {
    const date = new Date(isoString);
    return date.toLocaleString('ru-RU', {
        day: '2-digit',
        month: '2-digit',
        year: 'numeric',
        hour: '2-digit',
        minute: '2-digit',
        hour12: false,
    });
}

function LabStatusCard({ labId, submission, courseId }: LabStatusCardProps) {
    const [lab, setLab] = useState<Lab>();
    const navigate = useNavigate();

    useEffect(() => {
        const fetchLab = async () => {
            try {
                const labRes = await fetch(`http://localhost:3001/labs/${labId}`);
                const labData: Lab = await labRes.json();
                setLab(labData);

            } catch (error) {
                console.error("Ошибка при загрузке данных:", error);
            }
        };

        fetchLab();

    }, []);
    console.log(submission?.status);
    const labStatus: LabStatus = getSubmissionStatusClass(submission?.status || "")
    return (
        <div className={`lab-status-card ${labStatus.status}`}>
            <div className="lab-name">{lab?.name} {lab?.description}</div>
            <div className="lab-status-card-info">
                <div className="lab-info-row">
                    <label>Срок сдачи: </label>
                    <span className={`row-info ${labStatus.status}`}>{formatIsoString(lab?.deadline || "")}</span>
                </div>
                <div className="lab-info-row">
                    <label>Баллы за сдачу: </label>
                    <span className={`row-info ${labStatus.status}`}>{lab?.score}</span>
                </div>
                <div className="lab-info-row">
                    <label>Баллы за сдачу после дедлайна: </label>
                    <span className={`row-info ${labStatus.status}`}>{lab?.scoreAfterDeadline}</span>
                </div>
                <div className="lab-info-row">
                    <label>Статус: </label>
                    <span className={`row-info ${labStatus.status}`}>{labStatus.statusName}</span>
                </div>
                <div className="lab-buttons">
                    <button className="downoload-button">Скачать условие</button>
                    <button className={`sign-button ${labStatus.status}`} onClick={() => navigate(`/courses/${courseId}`)} disabled={labStatus.statusName === 'Сдана не в срок' || labStatus.statusName === 'Сдана'} >
                        {(labStatus.statusName === 'Сдана не в срок' || labStatus.statusName === 'Сдана') ? 'Запись недоступна' : 'Записаться'}
                    </button>
                </div>
            </div>
        </div>
    )
}

export default LabStatusCard;