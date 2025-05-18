// src/pages/Login/Login.tsx
import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import './LoginPage.css'

export default function Login() {
    const [email, setEmail] = useState('')
    const [password, setPassword] = useState('')
    const navigate = useNavigate()

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault()
        console.log('Вход:', { email, password })
        navigate('/student-profile')
    }

    return (
        <div className="auth-page">
            <div className='login-tittle'>Добро пожаловать!</div>
            <form className="auth-form" onSubmit={handleSubmit}>
                <label>
                    Логин
                    <input type="email" value={email} onChange={e => setEmail(e.target.value)} required />
                </label>
                <label>
                    Пароль
                    <input type="password" value={password} onChange={e => setPassword(e.target.value)} required />
                </label>
                <button type="submit">Войти</button>
            </form>
        </div>
    )
}
