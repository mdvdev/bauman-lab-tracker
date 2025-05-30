import React, { useEffect, useState } from 'react';
import "./EditProfile.css"
import { authFetch } from '../../utils/authFetch';
type EditProfileProps = {
    onSave: () => void;
    onClose: () => void;
}

const EditProfile: React.FC<EditProfileProps> = ({ onSave, onClose }) => {
    const [firstName, setFirstName] = useState('');
    const [lastName, setLastName] = useState('');
    const [patronymic, setPatronymic] = useState('');
    const [telegramUsername, setTelegramUsername] = useState('');
    const [email, setEmail] = useState('');

    useEffect(() => {
        authFetch('api/v1/users/me')
            .then((res) => res.json())
            .then((data) => {
                setEmail(data.email)
                setFirstName(data.firstName);
                setLastName(data.lastName);
                setPatronymic(data.patronymic);
                setTelegramUsername(data.telegramUsername);
            }
            )

    }, [])

    const formFields = {
        firstName,
        lastName,
        patronymic,
        telegramUsername,
        email
    };
    const isFormValid = formFields.firstName != "" && formFields.lastName != "" && formFields.email != "";
    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault(); // предотвратить перезагрузку страницы
        try {
            const res = await authFetch('api/v1/users/me', {
                method: 'PATCH',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    email,
                    firstName,
                    lastName,
                    patronymic,
                    telegramUsername,
                }),
            });

            if (!res.ok) {
                throw new Error('Ошибка обновления профиля');
            }

            const data = await res.json();
            console.log('Профиль обновлён:', data);
        } catch (error) {
            console.error(error);
        }
        onSave();
        onClose();
    };

    return (
        <form onSubmit={handleSubmit} className="edit-profile-form">
            <label>
                Имя
                <input
                    type="text"
                    value={firstName}
                    onChange={(e) => setFirstName(e.target.value)}
                />
            </label>

            <label>
                Фамилия
                <input
                    type="text"
                    value={lastName}
                    onChange={(e) => setLastName(e.target.value)}
                />
            </label>

            <label>
                Отчество
                <input
                    type="patronymic"
                    value={patronymic}
                    onChange={(e) => setPatronymic(e.target.value)}
                />
            </label>

            <label>
                Имя пользователя в telegramm
                <input
                    type="telegramUsername"
                    value={telegramUsername}
                    onChange={(e) => setTelegramUsername(e.target.value)}
                />
            </label>

            <label>
                Почта пользователя
                <input
                    type="email"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                />
            </label>

            <button type="submit" disabled={!isFormValid}>Готово</button>
        </form>
    );
};

export default EditProfile;
