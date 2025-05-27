import { Item } from "./itemType";

export type Notification = {
    items: Item[],
    totalCount: number,
    unreadCount: number
};