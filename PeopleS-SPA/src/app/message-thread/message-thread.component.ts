import { Component, OnInit, OnDestroy, Input, ElementRef, ViewChild } from '@angular/core';
import { MessageService } from '../_services/message.service';
import { Message } from '../_models/message';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { MessageToSend } from '../_models/messageToSend';
import { NgxSpinnerService } from 'ngx-spinner';

@Component({
  selector: 'app-message-thread',
  templateUrl: './message-thread.component.html',
  styleUrls: ['./message-thread.component.css']
})
export class MessageThreadComponent implements OnInit {

  secondUserId: number;
  messages: Message[] = null;
  content: string;

  currentPage = 1;

  notEmptyPost = true;
  notscrolly = true;

  constructor(private messageService: MessageService,
              private authService: AuthService,
              private alertifyService: AlertifyService,
              private spinner: NgxSpinnerService) { }

  ngOnInit(): void {
    this.messageService.currentThread$.subscribe( data => {

      if( data === null ) return;

      this.secondUserId = data;

      this.currentPage = 1;
      this.loadInitialMessages();

      this.notEmptyPost = true;
      this.notscrolly = true;
    });
  }

  loadInitialMessages() {
    this.messageService.GetMessageThread(this.secondUserId, 1, 10).subscribe( data => {
      this.messages = data.reverse();
    });
  }

  isMine(message: Message): boolean {

    if( message.senderId === this.secondUserId ) 
      return false;

    return true;
  }

  sendMessage() {
    const message: MessageToSend = {
      senderId: this.authService.getToken().nameId,
      recipientId: this.secondUserId,
      content: this.content
    };

    this.messageService.SendMessage(message)
      .subscribe(data => {
        this.messages.push(data);
        this.content = "";
    }, error => {
      this.alertifyService.error("Couldn't send message");
    });
  }

  onScrollUp() {
    console.log("Not scrolly: " + this.notscrolly);
    console.log("NotEmptyPost: " + this.notEmptyPost);
    if (this.notscrolly && this.notEmptyPost) {
      this.spinner.show();
      this.notscrolly = false;
      this.loadNextData();
   }
  }

  loadNextData() {
    this.currentPage += 1;
    console.log(this.currentPage);
    this.messageService.GetMessageThread(this.secondUserId, this.currentPage, 10)
    .subscribe( (data) => {
      if (data.length === 0 ) {
        this.notEmptyPost =  false;
      }

      for (const item of data) {
        this.messages.unshift(item);
      }

      this.spinner.hide();
      this.notscrolly = true;
    });
  }

}
