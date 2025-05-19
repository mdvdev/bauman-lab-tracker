import { useEffect, useState } from 'react';
import './UserCard.css';
import { User } from '../../types/userType';
import { translateRole } from '../../utils/roleUtils';
import { Calendar, Mail } from 'lucide-react';
import Modal from '../Modal/Modal';
import EditProfile from '../EditProfile/EditProfile';

function UserCard() {
    const [user, setUser] = useState<User | null>(null);
    const [isModalOpen, setIsModalOpen] = useState(false);

    const loadUser = async () => {
        try {
            const response = await fetch('http://localhost:3001/users/2');
            const data = await response.json();
            setUser(data);
        } catch (error) {
            console.error('Ошибка при загрузке пользователя:', error);
        }
    };

    useEffect(() => {
        loadUser();
    }, []);
    if (!user) return <div>Загрузка...</div>;

    const translatedRole = translateRole(user.role);

    return (
        <>
            <div className="card-wrapper">
                <div className="card-content">
                    <div className="user-info">
                        <img className="avatar" src={user.photo} alt="avatar" />
                        {user.firstName && user.lastName && (
                            <span className="user-name">
                                {user.firstName} {user.lastName} {user.patronymic}
                            </span>
                        )}
                        {user.role && (
                            <span className={`role-badge ${user.role}`}>
                                {translatedRole}
                            </span>
                        )}
                        <button className="edit-button" onClick={() => setIsModalOpen(true)}>
                            Редактировать
                        </button>
                    </div>

                    <div className="contact-info">
                        <div className="telegram-row">
                            <img className="telegram-icon" src="/icons/telegram.png" alt="telegram" />
                            {user.telegram && (
                                <span className="telegram-info">{user.telegram}</span>
                            )}
                        </div>
                        <div className="created-at-row">
                            <Calendar size={20} color="#6C757D" />
                            {user.createdAt && (
                                <span className="crated-at-info">
                                    {new Date(user.createdAt).toLocaleDateString()}
                                </span>
                            )}
                        </div>
                        <div className="email-row">
                            <Mail size={20} color="#6C757D" />
                            {user.email && (
                                <span className="crated-at-info">{user.email}</span>
                            )}
                        </div>
                        <div className="group-row">
                            <img className="group-icon" src="/icons/gmail_groups.png" alt="group" />
                        </div>
                    </div>
                </div>
            </div>

            {isModalOpen && (
                <Modal onClose={() => setIsModalOpen(false)}>
                    <EditProfile
                        onSave={loadUser}
                        onClose={() => setIsModalOpen(false)}
                    />
                </Modal>
            )}
        </>
    );
}

export default UserCard;
