export interface MessageThreadList {
    userOneId: number;
    userTwoId: number;
    userTwoName: string;
    userTwoSurname: string;
    userTwoPhotoUrl: string;
    lastModified: Date;
    content: string;
    lastMessageIsMine: boolean;
    isRead: boolean;
}