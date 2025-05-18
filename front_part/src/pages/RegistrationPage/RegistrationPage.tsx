// src/pages/Register/Register.tsx
import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import './RegistrationPage.css'

export default function Register() {
    const [email, setEmail] = useState('')
    const [password, setPassword] = useState('')
    const navigate = useNavigate()

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault()
        console.log('Регистрация:', { email, password })
        navigate('/login')
    }

    return (
        <div className="auth-page">
            <h2>Регистрация</h2>
            <form className="auth-form" onSubmit={handleSubmit}>
                <label>
                    Email
                    <input type="email" value={email} onChange={e => setEmail(e.target.value)} required />
                </label>
                <label>
                    Пароль
                    <input type="password" value={password} onChange={e => setPassword(e.target.value)} required />
                </label>
                <button type="submit">Зарегистрироваться</button>
            </form>
        </div>
    )
}
