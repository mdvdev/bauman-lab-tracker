import { LabStatus } from "../types/labStatusType";

export function getSubmissionStatusClass(status: string): LabStatus {
    switch (status) {
        case 'Approved':
            return { statusName: 'Сдана', status: 'status-approved' };
        case 'Rejected':
            return { statusName: 'Не сдана', status: 'status-rejected' };
        case 'NeedsRevision':
            return { statusName: 'Не сдана', status: 'status-needs-revision' };
        case 'InProgress':
            return { statusName: 'Не сдана', status: 'status-in-progress' };
        case 'Approved after deadline':
            return { statusName: 'Сдана не в срок', status: 'status-approved-after-deadline' };
        case '':
            return { statusName: 'Не сдана', status: 'status-not-signed' };
        default:
            return { statusName: 'Не сдана', status: 'status-in-progress' };
    }
}