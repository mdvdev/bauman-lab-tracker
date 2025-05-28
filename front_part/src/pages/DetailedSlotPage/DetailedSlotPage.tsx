import { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import { Slot } from '../../types/slotType';
import { Submission } from '../../types/submssionType';
import { Course } from '../../types/courseType';
import './DetailedSlotPage.css';
import { authFetch } from '../../utils/authFetch';

interface SubmissionDisplay {
    submissionId: string;
    fullName: string;
    labName: string;
    statusRaw: string;
    status: string; 
    score?: number;
    scoreRegular?: number;
    scoreAfterDeadline?: number;
}

function getStatusLabel(status: string) {
    switch (status) {
        case 'approved': return 'Принято';
        case 'approved after deadline': return 'Принято не в срок';
        case 'rejected': return 'Отклонено';
        default: return status;
    }
}

function DetailedSlotPage() {
    const { slotId, courseId } = useParams<{ slotId: string; courseId: string }>();
    const [slot, setSlot] = useState<Slot | null>(null);
    const [submissions, setSubmissions] = useState<SubmissionDisplay[]>([]);
    const [courseName, setCourseName] = useState<string>('');

    const [showModal, setShowModal] = useState(false);
    const [selectedSubmission, setSelectedSubmission] = useState<SubmissionDisplay | null>(null);
    const [comment, setComment] = useState('');
    const [status, setStatus] = useState<'approved' | 'rejected' | 'approved after deadline'>('approved');

    useEffect(() => {
        const loadData = async () => {
            try {
                if (!slotId || !courseId) return;

                const slotRes = await authFetch(`/api/v1/courses/${courseId}/slots/${slotId}`);
                if (!slotRes.ok) throw new Error('Ошибка загрузки слота');
                const slotData: Slot = await slotRes.json();
                setSlot(slotData);

                const submissionsRes = await authFetch(`/api/v1/courses/${courseId}/submissions`);
                if (!submissionsRes.ok) throw new Error('Ошибка загрузки заявок');
                const submissionsData: Submission[] = await submissionsRes.json();

                const filtered = submissionsData.filter(sub => sub.slot?.id === slotId);

                const result: SubmissionDisplay[] = filtered.map(sub => {
                    const fullName = `${sub.student?.lastName ?? ''} ${sub.student?.firstName ?? ''} ${sub.student?.patronymic ?? ''}`.trim();
                    // Заменяем статус в объектах Submission, если там "approved_after_deadline", меняем на "approved after deadline"
                    let rawStatus = sub.submissionStatus === 'approved_after_deadline' ? 'approved after deadline' : sub.submissionStatus;
                    const statusLabel = getStatusLabel(rawStatus);

                    return {
                        submissionId: sub.id,
                        fullName,
                        labName: sub.lab?.name ?? '',
                        statusRaw: rawStatus,
                        status: statusLabel,
                        scoreRegular: sub.lab?.score,
                        scoreAfterDeadline: sub.lab?.scoreAfterDeadline,
                    };
                });

                setSubmissions(result);

                const courseRes = await authFetch(`/api/v1/courses/${courseId}`);
                if (courseRes.ok) {
                    const courseData: Course = await courseRes.json();
                    setCourseName(courseData.name);
                }
            } catch (error) {
                console.error("Ошибка при загрузке данных:", error);
            }
        };

        loadData();
    }, [slotId, courseId]);

    const handleAccept = (submissionId: string) => {
        const target = submissions.find(s => s.submissionId === submissionId);
        if (!target) return;
        setSelectedSubmission(target);
        setComment('');
        setStatus('approved');
        setShowModal(true);
    };

    const handleConfirmAccept = async () => {
        if (!selectedSubmission || !courseId) return;

        const chosenScore = status === 'approved'
            ? selectedSubmission.scoreRegular
            : status === 'approved after deadline'
                ? selectedSubmission.scoreAfterDeadline
                : 0;

        try {
            const res = await authFetch(
                `/api/v1/courses/${courseId}/submissions/${selectedSubmission.submissionId}/status`,
                {
                    method: 'PATCH',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({
                        submissionStatus: status,
                        score: chosenScore,
                        comment,
                    }),
                }
            );

            if (!res.ok) throw new Error(`Ошибка обновления заявки: ${res.status}`);

            setSubmissions(prev =>
                prev.map(s =>
                    s.submissionId === selectedSubmission.submissionId
                        ? {
                              ...s,
                              statusRaw: status,
                              status: getStatusLabel(status),
                              score: chosenScore,
                          }
                        : s
                )
            );

            setShowModal(false);
            setSelectedSubmission(null);
            setComment('');
            setStatus('approved');
        } catch (err) {
            console.error('Ошибка при подтверждении:', (err as Error).message);
        }
    };

    const handleRemove = async (submissionId: string) => {
        if (!courseId) return;
        const confirmed = window.confirm('Вы уверены, что хотите удалить эту заявку?');
        if (!confirmed) return;

        try {
            const res = await authFetch(
                `/api/v1/courses/${courseId}/submissions/${submissionId}`,
                {
                    method: 'DELETE',
                    headers: { 'Content-Type': 'application/json' },
                }
            );

            if (!res.ok) throw new Error(`Ошибка удаления заявки: ${res.status}`);

            setSubmissions(prev => prev.filter(s => s.submissionId !== submissionId));
        } catch (err) {
            console.error('Ошибка при удалении:', (err as Error).message);
        }
    };

    if (!slot) return <div>Загрузка слота...</div>;

    const formatDate = (iso: string) =>
        new Date(iso).toLocaleDateString('ru-RU', {
            year: 'numeric',
            month: 'long',
            day: 'numeric',
            timeZone: 'UTC'
        });

    const formatTime = (iso: string) =>
        new Date(iso).toLocaleTimeString('ru-RU', {
            hour: '2-digit',
            minute: '2-digit',
            timeZone: 'UTC'
        });

    return (
        <div className="slot-page">
            <div className="slot-header">
                <h2>{formatDate(slot.startTime)}</h2>
                <p className="slot-course"><strong>Курс:</strong> {courseName}</p>
                <div className="slot-time-columns">
                    <div className="slot-time-labels">
                        <p><strong>Начало:</strong></p>
                        <p><strong>Конец:</strong></p>
                    </div>
                    <div className="slot-time-values">
                        <p>{formatTime(slot.startTime)}</p>
                        <p>{formatTime(slot.endTime)}</p>
                    </div>
                </div>
            </div>

            <div className="students-list">
                {submissions.map((s, index) => {
                    const isAccepted =
                        s.statusRaw === 'approved' || s.statusRaw === 'approved after deadline';

                    return (
                        <div key={s.submissionId} className="student-row">
                            <div><strong>{index + 1}.</strong></div>
                            <div>{s.fullName}</div>
                            <div>{s.labName}</div>
                            <div>
                                <div>{s.status}</div>
                            </div>
                            <div className="actions">
                                <button
                                    className="accept-btn"
                                    onClick={() => handleAccept(s.submissionId)}
                                    style={{
                                        backgroundColor: isAccepted ? '#9e9e9e' : '#1A1B22'
                                    }}
                                >
                                    {isAccepted ? 'Изменить' : 'Принять'}
                                </button>
                                <button className="remove-btn" onClick={() => handleRemove(s.submissionId)}>Удалить</button>
                            </div>
                        </div>
                    );
                })}
            </div>

            {showModal && selectedSubmission && (
                <div className="modal-overlay">
                    <div className="modal">
                        <h3>Подтверждение заявки</h3>

                        <label>Комментарий:</label>
                        <textarea
                            value={comment}
                            onChange={e => setComment(e.target.value)}
                        />

                        <label>Статус:</label>
                        <select
                            value={status}
                            onChange={e => setStatus(e.target.value as typeof status)}
                        >
                            <option value="approved">Принято</option>
                            <option value="approved after deadline">Принято не в срок</option>
                            <option value="rejected">Отклонено</option>
                        </select>

                        <div className="modal-buttons">
                            <button onClick={handleConfirmAccept}>Принять</button>
                            <button onClick={() => setShowModal(false)}>Отмена</button>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
}

export default DetailedSlotPage;