import React, { useEffect, useState } from 'react';
import "./EditProfile.css"
type EditProfileProps = {
    onSave: () => void;
    onClose: () => void;
}

const EditProfile: React.FC<EditProfileProps> = ({ onSave, onClose }) => {
    const [firstName, setFirstName] = useState('');
    const [lastName, setLastName] = useState('');
    const [email, setEmail] = useState('');
    const [oldPassword, setOldPassword] = useState('');
    const [newPassword, setNewPassword] = useState('');

    useEffect(() => {
        fetch('http://localhost:5272/api/v1/users/me')
            .then((res) => res.json())
            .then((data) => {
                setFirstName(data.firstName);
                setLastName(data.lastName);
                setEmail(data.email);
                setOldPassword(data.oldPassword);
                setNewPassword(data.newPassword);
            }
            )

    }, [])

    const formFields = {
        firstName,
        lastName,
        email,
        oldPassword,
        newPassword,
    };
    const isFormValid = formFields.firstName != "" && formFields.lastName != "" && formFields.email != "";
    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault(); // предотвратить перезагрузку страницы
        try {
            const res = await fetch('http://localhost:5272/api/v1/users/me', {
                method: 'PATCH',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    firstName,
                    lastName,
                    email,
                    // password: newPassword, // если хочешь отправлять новый пароль
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
                Почта
                <input
                    type="email"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                />
            </label>

            <label>
                Старый пароль
                <input
                    type="password"
                    value={oldPassword}
                    onChange={(e) => setOldPassword(e.target.value)}
                />
            </label>

            <label>
                Новый пароль
                <input
                    type="password"
                    value={newPassword}
                    onChange={(e) => setNewPassword(e.target.value)}
                />
            </label>
            <button type="submit" disabled={!isFormValid}>Готово</button>
        </form>
    );
};

export default EditProfile;
