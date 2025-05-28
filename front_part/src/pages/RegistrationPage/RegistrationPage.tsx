// src/pages/Register/Register.tsx
import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import './RegistrationPage.css'
import { Link } from 'react-router-dom'
export default function Register() {
    const [email, setEmail] = useState('')
    const [password, setPassword] = useState('')
    const [firstName, setFirstName] = useState('')
    const [lastName, setLastName] = useState('')
    const [patronymic, setPatronymic] = useState('')
    const [group, setGroup] = useState('')



    const navigate = useNavigate()

    const handleRegister = async (e: React.FormEvent) => {
        e.preventDefault();
        const res = await fetch('/api/v1/auth/register/', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ email, password, firstName, lastName, group, patronymic }),
        });
        if (res.ok) {
            navigate('/login');
        } else {

        }
    };
    return (
        <div className="auth-page">
            <h2>Регистрация</h2>
            <form className="auth-form" onSubmit={handleRegister}>
                <label>
                    Email
                    <input type="email" value={email} onChange={e => setEmail(e.target.value)} required />
                </label>
                <label>
                    Пароль
                    <input type="password" value={password} onChange={e => setPassword(e.target.value)} required />
                </label>
                <label>
                    Имя
                    <input type="firstName" value={firstName} onChange={e => setFirstName(e.target.value)} required />
                </label>
                <label>
                    Фамилия
                    <input type="lastName" value={lastName} onChange={e => setLastName(e.target.value)} required />
                </label>
                <label>
                    Отчество
                    <input type="patronymic" value={patronymic} onChange={e => setPatronymic(e.target.value)} required />
                </label>
                <label>
                    Группа
                    <input type="group" value={group} onChange={e => setGroup(e.target.value)} required />
                </label>
                <button type="submit">Зарегистрироваться</button>
                <Link to="/login" className='login-link'>Уже есть аккаунт?</Link>
            </form>
        </div>
    )
}
