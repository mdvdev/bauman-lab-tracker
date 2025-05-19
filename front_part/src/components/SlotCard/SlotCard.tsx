import "./SlotCard.css"
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faUser } from '@fortawesome/free-solid-svg-icons';
import { Slot } from '../../types/slotType'
import Modal from "../Modal/Modal";
import { useState } from "react";
import LabSelection from "../LabSelection/LabSelection"
import { useEffect } from "react";
function formatTime(timeString: string): string {
    const date = new Date(timeString);
    return date.toLocaleTimeString([], {
        hour: '2-digit',
        minute: '2-digit',
        hour12: false
    });
}
function SlotCard(props: { slot: Slot }) {
    const { slot } = props;
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [modalType, setModalType] = useState<'signSlot' | 'slotParticipant' | null>(null);
    const [signSuccess, setSignSuccess] = useState<boolean>(false);
    const [error, setError] = useState<string | null>(null);
    const [submissionId, setSubmissionId] = useState<number | null>(null);

    useEffect(() => {
        const checkSubmission = async () => {
            try {
                const res = await fetch(`http://localhost:3001/submissions?userId=1&slotId=${slot.id}`);
                const data = await res.json();
                if (data.length > 0) {
                    setSignSuccess(true);
                    setSubmissionId(data[0].id);
                }
            } catch (error) {
                console.error("Ошибка при проверке записи:", error);
            }
        };

        checkSubmission();
    }, [slot.id]);
    const deleteRecord = async () => {
        try {
            await fetch(`http://localhost:3001/submissions/${submissionId}`, {
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
                        <span className="slot-value">{slot.maxStudents - slot.currentStudents}</span>
                    </div>
                </div>
                <div className="slot-buttons">
                    <button className={`slot-button-sign ${signSuccess ? 'remove' : ''} ${(slot.maxStudents - slot.currentStudents) <= 0 ? 'disabled' : ''}`} disabled={(!signSuccess && (slot.maxStudents - slot.currentStudents) <= 0)} onClick={() => {
                        setIsModalOpen(true);
                        if (signSuccess) {
                            deleteRecord();
                            setIsModalOpen(false);
                        } else {
                            setModalType("signSlot");
                        }
                    }}>
                        {((slot.maxStudents - slot.currentStudents) <= 0 && !signSuccess) ? "Запись недоступна" : signSuccess ? "Убрать запись" : "Записаться"}
                    </button>
                    <button className="slot-button-participant" onClick={() => { setIsModalOpen(true); setModalType("slotParticipant") }}>
                        <FontAwesomeIcon icon={faUser} style={{ color: 'white' }} />
                    </button>
                </div>
            </div >
            {isModalOpen &&
                <Modal onClose={() => setIsModalOpen(false)}>
                    {modalType === 'signSlot' && <LabSelection courseId="1" userId="1" slotId={slot.id} onClose={() => setIsModalOpen(false)} successSign={(id) => { setSignSuccess(true); setSubmissionId(id) }}  ></LabSelection>}
                    {modalType === 'slotParticipant'}
                </Modal >
            }
        </>

    );
}
export default SlotCard;