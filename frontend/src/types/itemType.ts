export type Item = {
    id: string,
    userId: string,
    title: string,
    message: string,
    type: string,
    isRead: boolean,
    createdAt: string,
    relatedEntityId: string,
    relatedEntityType: string
};