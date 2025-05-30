import { useContext } from 'react';
import { Navigate } from 'react-router-dom';
import { AuthContext } from './AuthContext';
import { JSX } from 'react';

type ProtectedRouteProps = {
    children: JSX.Element;
};

const ProtectedRoute = ({ children }: ProtectedRouteProps) => {
    const { isAuthenticated, isLoading } = useContext(AuthContext);

    if (isLoading) {
        // Показываем загрузку, пока не проверим авторизацию
        return <div>Загрузка...</div>;
    }

    if (!isAuthenticated) {
        return <Navigate to="/login" replace />;
    }

    return children;
};

export default ProtectedRoute;
