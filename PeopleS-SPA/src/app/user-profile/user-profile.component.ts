import { Component, OnInit, TemplateRef } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';
import { ActivatedRoute } from '@angular/router';
import { UserService } from '../_services/user.service';
import { AuthService } from '../_services/auth.service';
import { Post } from 'src/app/_models/post';
import { map } from 'rxjs/operators';
import { UserProfile } from '../_models/user-profile';
import { User } from '../_models/User';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { MessageService } from '../_services/message.service';
import { MessageToSend } from '../_models/messageToSend';

@Component({
  selector: 'app-user-profile',
  templateUrl: './user-profile.component.html',
  styleUrls: ['./user-profile.component.css']
})
export class UserProfileComponent implements OnInit {

  modalRef: BsModalRef;
  visible: boolean[] = [true, false, false];
  user: User;
  allpost: Post[];
  notEmptyPost = true;
  notscrolly = true;
  currentPage: number;
  content = '';
  emptyMessage = false;
  isMe = false;

 constructor(
    private route: ActivatedRoute,
    private spinner: NgxSpinnerService,
    private userService: UserService,
    private authService: AuthService,
    private modalService: BsModalService,
    private messageService: MessageService
  ) {
}

  ngOnInit() {
    this.route.data.pipe(
    ).subscribe( data => {
      this.allpost = data.userprofile.posts;
      this.user = data.userprofile.user;
      if (this.user.id.toString() === this.authService.getToken().nameid) {
        this.isMe = true;
      }
    });
    this.currentPage = 1;
}

onScroll() {
  if (this.notscrolly && this.notEmptyPost) {
    this.spinner.show();
    this.notscrolly = false;
    this.loadNextPost();
 }
}

loadNextPost() {
  this.currentPage += 1;

  this.userService.getUserPosts( this.authService.getToken().nameid, this.currentPage)
  .pipe( map<UserProfile, Post[]>(data => data.posts) )
  .subscribe( (data: Post[]) => {

    if (data.length === 0 ) {
      this.notEmptyPost =  false;
    }

    console.log(this.allpost);

    for (const item of data) {
      this.allpost.push(item);
    }

    console.log(this.allpost);

    this.spinner.hide();
    this.notscrolly = true;
  });
}

menu(choice: number) {
  this.visible = [false, false, false];
  this.visible[choice] = true;
}

postsRecieved(): boolean{
  return !(Object.keys(this.allpost).length === 0);
}

createModal( template: TemplateRef<any> ) {
  this.emptyMessage = false;
  this.modalRef = this.modalService.show(template);
}

sendMessage() {
  if (this.content.length === 0)
  {
    this.emptyMessage = true;
    return;
  }

  const message: MessageToSend = {
    senderId: this.authService.getToken().nameid,
    recipientId: this.user.id,
    content: this.content
  };

  this.messageService.sendMessage(message).subscribe();
  this.modalRef.hide();
}

}
