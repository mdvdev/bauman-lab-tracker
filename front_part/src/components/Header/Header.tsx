import './Header.css'
import { Anchor } from 'lucide-react';

function Header() {
    return (
        <header className="header">
            <Anchor size={40} color="white" />

            <nav className="nav">
                <a href="/courses">Ваши курсы</a>
                <a href="/notifications">Уведомления</a>
                <a href="/student-profile">Профиль</a>
            </nav>

        </header>
    )
}

export default Header
