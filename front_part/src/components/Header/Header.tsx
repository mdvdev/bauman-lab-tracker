import './Header.css';
import { Anchor } from 'lucide-react';
import { authFetch } from '../../utils/authFetch';

function Header() {
    const logout = async () => {
        try {
            const response = await authFetch('/api/v1/auth/logout', {
                method: 'POST',
            });

            if (!response.ok) {
                console.error('Ошибка при выходе из аккаунта:', response.status);
            }

            // Удаление сохранённых данных и редирект
            localStorage.removeItem('basicCreds');
            window.location.href = '/login';

        } catch (error) {
            console.error('Ошибка при выходе из аккаунта:', error);
        }
    };

    return (
        <header className="header">
            <Anchor size={40} color="white" />

            <nav className="nav">
                <a href="/courses">Ваши курсы</a>
                <a href="/notifications">Уведомления</a>
                <a href="/student-profile">Профиль</a>
            </nav>

            <button className="logout" onClick={logout}>Выход</button>
        </header>
    );
}

export default Header;
