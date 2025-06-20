import { useEffect, useState } from "react";
import { Notification } from "../../types/notificationType";
import NotificationCard from "../../components/NotificationCard/NotificationCard";
import "./NotificationPage.css"
import { authFetch } from "../../utils/authFetch";
function NotificationPage() {
    const [notifications, setNotifications] = useState<Notification>()
    useEffect(() => {
        authFetch(`/api/v1/notifications`)
            .then((notificationsRes) => notificationsRes.json())
            .then((notificationsData: Notification) => setNotifications(notificationsData))
    }, []);
    return (
        <div className="page">
            <h2 className="notification-page-tittle">Последние уведомления</h2>
            <div className="notification-grid">
                {notifications?.items.length! === 0 ? "У вас пока нет новых уведомлений." : (
                    <div className="notification-list">
                        {notifications?.items.map(
                            (notification) => <NotificationCard notificationId={notification.id} ></NotificationCard>)
                        }
                    </div>)}
            </div>
        </div>
    )
}
export default NotificationPage;
