import { useEffect, useState } from "react";
import { Notification } from "../../types/notificationType";
import NotificationCard from "../../components/NotificationCard/NotificationCard";
import "./NotificationPage.css"
function NotificationPage() {
    const [notifications, setNotifications] = useState<Notification>()
    useEffect(() => {
        fetch(`/api/v1/notifications`)
            .then((notificationsRes) => notificationsRes.json())
            .then((notificationsData: Notification) => setNotifications(notificationsData))
    });
    return (
        <div className="page">
            <h2 className="notification-page-tittle">Последние уведомления</h2>
            <div className="notification-list">
                {notifications?.items.map(
                    (notification) => <NotificationCard notificationId={notification.id} ></NotificationCard>)
                }
            </div>
        </div>
    )
}
export default NotificationPage;
