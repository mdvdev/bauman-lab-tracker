import { useEffect, useState } from "react"
import { Item } from "../../types/itemType";
import { User } from "../../types/userType";
import "./NotificationCard.css"
import { authFetch } from "../../utils/authFetch";
type NotificationCardProps = {
    notificationId: string;
}

const NotificationCard: React.FC<NotificationCardProps> = ({ notificationId }) => {
    const [notification, setNotification] = useState<Item>();
    const [senderInfo, setSenderInfo] = useState<User>();

    useEffect(() => {
        const fetchNotification = async () => {
            try {
                const notificationRes = await authFetch(`/api/v1/notifications/${notificationId}`);
                if (!notificationRes.ok) throw new Error("Ошибка загрузки уведомления");

                const notificationData: Item = await notificationRes.json();
                setNotification(notificationData);

                if (notificationData.userId) {
                    const senderInfoRes = await authFetch(`/api/v1/users/${notificationData.userId}`);
                    if (!senderInfoRes.ok) throw new Error("Ошибка загрузки отправителя");

                    const senderInfoData: User = await senderInfoRes.json();
                    setSenderInfo(senderInfoData);
                }
            } catch (err) {
                console.error("Ошибка загрузки данных уведомления:", err);
            }
        };

        fetchNotification();
    }, [notificationId]);

    return (
        <div className="notification-card">
            <div className="notification-title">{notification?.title}</div>
            <div className="notification-info">
                <div>{notification?.message}</div>
                <div className="notification-sender-info">
                    <img src={`http://localhost:5272${senderInfo?.photoUri}`} className="sender-photo"></img>
                    <div>
                        <div>{senderInfo?.lastName} {senderInfo?.firstName}</div>
                        <div>{new Date(notification?.createdAt!).toLocaleDateString()}</div>
                    </div>
                </div>
            </div>
        </div >
    )
}

export default NotificationCard