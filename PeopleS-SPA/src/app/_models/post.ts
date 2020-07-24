import { PostUser } from './post-user';

export interface Post {
    id: number;
    text: string;
    photo: string;
    dateOfCreation: Date;
    userId: number;
    user: PostUser;
}
