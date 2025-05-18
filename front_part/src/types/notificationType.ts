export type Notification = {
    total: number;
    unreadCount: number;
    items: {
        id: string;
        type: string;
        title: string;
        message: string;
        isRead: boolean;
        metadata: {
            labId: string;
            courseId: string;
            submissionId: string;
        };
        createdAt: string;
    }[];
};