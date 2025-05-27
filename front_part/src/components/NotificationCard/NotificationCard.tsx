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
        authFetch(`/api/v1/notifications/${notificationId}`)
            .then((notificationRes) => notificationRes.json())
            .then((notificationData: Item) => setNotification(notificationData))

        authFetch(`/api/v1/users/${notification?.userId}`)
            .then((senderInfoRes) => senderInfoRes.json())
            .then((senderInfoData: User) => setSenderInfo(senderInfoData))

    })
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