import { useContext, useState } from 'react';
import { AuthContext } from '../../AuthContext';
import { useNavigate } from 'react-router-dom';
import "./LoginPage.css"
const Login = () => {
    const { login } = useContext(AuthContext);
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const navigate = useNavigate();

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        const success = await login(email, password);
        if (success) {
            navigate('/student-profile');
        } else {
            alert('Неверный email или пароль');
        }
    };

    return (
        <div className="auth-page">
            <div className="login-tittle">Добро пожаловать!</div>
            <form className="auth-form" onSubmit={handleSubmit}>
                <label>
                    Логин
                    <input
                        type="email"
                        value={email}
                        onChange={e => setEmail(e.target.value)}
                        required
                    />
                </label>
                <label>
                    Пароль
                    <input
                        type="password"
                        value={password}
                        onChange={e => setPassword(e.target.value)}
                        required
                    />
                </label>
                <button type="submit">Войти</button>
            </form>
        </div>
    );
};

export default Login;
