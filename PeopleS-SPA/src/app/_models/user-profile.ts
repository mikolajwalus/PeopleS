import { Post } from './post';
import { User } from './User';

export interface UserProfile {
    posts: Post[];
    user: User;
}
