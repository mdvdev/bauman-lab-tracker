import { useEffect, useState } from "react";
import { Lab } from "../../types/labType";
import './LabStatusCard.css'
import { getSubmissionStatusClass } from "../../utils/labStatusUtil"
import { Submission } from "../../types/submssionType"
import { LabStatus } from "../../types/labStatusType";
import { useNavigate } from 'react-router-dom';
import { authFetch } from "../../utils/authFetch";
import { User } from "../../types/userType";
interface LabStatusCardProps {
    labId: string;
    courseId: string | null;
}


function formatIsoString(isoString: string): string {
    const date = new Date(isoString);
    return date.toLocaleString('ru-RU', {
        minute: '2-digit',
        hour12: false,
        day: '2-digit',
        month: '2-digit',
        year: 'numeric',
        hour: '2-digit'
    });
}

function LabStatusCard({ labId, courseId }: LabStatusCardProps) {
    const [lab, setLab] = useState<Lab>();
    const navigate = useNavigate();
    const [submission, setSubmission] = useState<Submission | null>(null);
    const [myUserInfo, setMyUserInfo] = useState<User>();
    const [isAdmOrTeacher, setIsAdmOrTeacher] = useState<boolean>();
    useEffect(() => {
        const fetchLab = async () => {
            try {
                const labRes = await authFetch(`/api/v1/courses/${courseId}/labs/${labId}`);
                const labData: Lab = await labRes.json();
                console.log(labData)
                setLab(labData);

                const submissionsRes = await authFetch(`/api/v1/courses/${courseId}/submissions/me`);
                if (!submissionsRes.ok) throw new Error("Не удалось загрузить данные о сдачах");
                const submissionsData: Submission[] = await submissionsRes.json();
                console.log(submissionsData)

                const myUserInfoRes = await authFetch(`/api/v1/users/me`);
                const myUserInfoData: User = await myUserInfoRes.json();
                setMyUserInfo(myUserInfo)
                myUserInfoData && setIsAdmOrTeacher(myUserInfoData.roles.includes('Administrator') || myUserInfoData.roles.includes('Teacher'));

                console.log(isAdmOrTeacher);
                const labSubmission = submissionsData.find(sub => sub.lab.id === labId);
                setSubmission(labSubmission || null);

            } catch (error) {
                console.error("Ошибка при загрузке данных:", error);
            }
        };

        fetchLab();

    }, []);
    const labStatus: LabStatus = getSubmissionStatusClass(submission?.submissionStatus || "")
    return (
        <div className={`lab-status-card ${labStatus.status}`}>
            <div className="lab-name">{lab?.name} {lab?.descriptionUri}</div>
            <div className="lab-status-card-info">
                <div className="lab-info-row">
                    <label>Срок сдачи: </label>
                    <span className={`row-info ${labStatus.status}`}>{formatIsoString(lab?.deadline || "")}</span>
                </div>
                <div className="lab-info-row">
                    <label>Баллы(в срок): </label>
                    <span className={`row-info ${labStatus.status}`}>{lab?.score}</span>
                </div>
                <div className="lab-info-row">
                    <label>Баллы(не в срок): </label>
                    <span className={`row-info ${labStatus.status}`}>{lab?.scoreAfterDeadline}</span>
                </div>
                <div className="lab-info-row">
                    <label>Статус: </label>
                    <span className={`row-info ${labStatus.status}`}>{labStatus.statusName}</span>
                </div>
                {isAdmOrTeacher
                    ? <button className="lab-refactor-button">Редактировать</button>
                    : (<div className="student-lab-buttons">
                        <button className="downoload-button">Скачать условие</button>
                        <button className={`sign-button ${labStatus.status}`} onClick={() => navigate(`/courses/${courseId}`)} disabled={labStatus.statusName === 'Сдана не в срок' || labStatus.statusName === 'Сдана'} >
                            {(labStatus.statusName === 'Сдана не в срок' || labStatus.statusName === 'Сдана') ? 'Запись недоступна' : 'Записаться'}
                        </button>
                    </div>)}
            </div>
        </div>
    )
}
export default LabStatusCard;
