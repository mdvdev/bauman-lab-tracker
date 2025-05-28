import "./SlotCard.css";
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faUser } from '@fortawesome/free-solid-svg-icons';
import { Slot } from '../../types/slotType';
import Modal from "../Modal/Modal";
import { useState, useEffect } from "react";
import LabSelection from "../LabSelection/LabSelection";
import { authFetch } from "../../utils/authFetch";
import { Submission } from "../../types/submssionType";
import { User } from "../../types/userType";
import { useNavigate } from "react-router-dom";

function formatTime(timeString: string): string {
    const date = new Date(timeString);
    return date.toLocaleTimeString([], {
        hour: '2-digit',
        minute: '2-digit',
        hour12: false,
    });
}

function SlotCard({ slot, courseId, userId }: { slot: Slot; courseId: string; userId: string }) {
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [modalType, setModalType] = useState<'signSlot' | 'slotParticipant' | null>(null);
    const [signSuccess, setSignSuccess] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [submissionId, setSubmissionId] = useState<number | null>(null);
    const [slotSubmissions, setSlotSubmissions] = useState<Submission[]>([]);
    const [isAdmOrTeacher, setIsAdmOrTeacher] = useState<boolean>(false);
    const navigate = useNavigate();
    useEffect(() => {
        const checkSubmission = async () => {
            try {
                const mySubRes = await authFetch(`/api/v1/courses/${courseId}/submissions/me`);
                const mySubData = await mySubRes.json();

                if (mySubData.length > 0) {
                    setSignSuccess(true);
                    setSubmissionId(mySubData[0].id);
                }

                const userRes = await authFetch(`/api/v1/users/me`);
                const userData: User = await userRes.json();
                setIsAdmOrTeacher(userData.roles.includes('Administrator') || userData.roles.includes('Teacher'));

                const slotSubsRes = await authFetch(`/api/v1/courses/${courseId}/submissions/?slotId=${slot.id}`);
                const slotSubsData = await slotSubsRes.json();
                setSlotSubmissions(slotSubsData);
            } catch (error) {
                console.error("Ошибка при проверке записи:", error);
            }
        };

        checkSubmission();
    }, [slot.id, courseId]);

    const deleteRecord = async () => {
        try {
            await authFetch(`/api/v1/courses/${courseId}/submissions/${submissionId}`, {
                method: 'DELETE',
                headers: {
                    'Content-Type': 'application/json',
                },
            });
        } catch (err) {
            setError((err as Error).message);
        }
        setSignSuccess(false);
        setSubmissionId(null);
    };

    const remainingSlots = Number(slot.maxStudents) - slotSubmissions.length;
    const isFull = remainingSlots <= 0;

    return (
        <>
            <div className="slot-card">
                <div className="slot-information">
                    <div className="slot-date">{new Date(slot.startTime).toLocaleDateString()}</div>

                    <div className="slot-row">
                        <span className="slot-label">Начало:</span>
                        <span className="slot-value">{formatTime(slot.startTime)}</span>
                    </div>

                    <div className="slot-row">
                        <span className="slot-label">Конец:</span>
                        <span className="slot-value">{formatTime(slot.endTime)}</span>
                    </div>

                    <div className="slot-row">
                        <span className="slot-label">Всего мест:</span>
                        <span className="slot-value">{slot.maxStudents}</span>
                    </div>

                    <div className="slot-row">
                        <span className="slot-label">Осталось:</span>
                        <span className="slot-value">{Math.max(0, remainingSlots)}</span>
                    </div>
                </div>

                {isAdmOrTeacher ? (
                    <button
                    className={`slot-teachers-button ${isFull ? 'full' : 'available'}`}
                    onClick={() => navigate(`/courses/${courseId}/slots/${slot.id}`)}
                >
                    {isFull ? "Посмотреть итоги" : "Перейти к слоту"}
                </button>
                ) : (
                    <div className="slot-students-buttons">
                        <button
                            className={`slot-button-sign ${signSuccess ? 'remove' : ''} ${isFull && !signSuccess ? 'disabled' : ''}`}
                            disabled={!signSuccess && isFull}
                            onClick={() => {
                                setIsModalOpen(true);
                                if (signSuccess) {
                                    deleteRecord();
                                    setIsModalOpen(false);
                                } else {
                                    setModalType("signSlot");
                                }
                            }}
                        >
                            {isFull && !signSuccess
                                ? "Запись недоступна"
                                : signSuccess
                                    ? "Убрать запись"
                                    : "Записаться"}
                        </button>
                        <button
                            className="slot-button-participant"
                            onClick={() => {
                                setIsModalOpen(true);
                                setModalType("slotParticipant");
                            }}
                        >
                            <FontAwesomeIcon icon={faUser} style={{ color: 'white' }} />
                        </button>
                    </div>
                )}
            </div>

            {isModalOpen && (
                <Modal onClose={() => setIsModalOpen(false)}>
                    {modalType === 'signSlot' && (
                        <LabSelection
                            courseId={courseId}
                            userId={userId}
                            slotId={slot.id}
                            onClose={() => setIsModalOpen(false)}
                            successSign={(id) => {
                                setSignSuccess(true);
                                setSubmissionId(id);
                            }}
                        />
                    )}
                    {modalType === 'slotParticipant' && (
                        <div>Здесь будет список участников слота</div>
                    )}
                </Modal>
            )}
        </>
    );
}

export default SlotCard;
