import { Component, OnInit} from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MessageThreadList } from '../_models/message-thread-list';
import { MessageService } from '../_services/message.service';
import { NgxSpinnerService } from 'ngx-spinner';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {

  constructor(private route: ActivatedRoute,
              private messageService: MessageService,
              private spinner: NgxSpinnerService) { }

  threads: MessageThreadList[];
  componentRef: any;

  notEmptyPost = true;
  notscrolly = true;

  currentPage = 1;

  firstTime = true;

  ngOnInit(): void {
    this.route.data.subscribe( data => {
      this.threads = data.messanger;
      this.loadComponent(this.threads[0].userTwoId);
      console.log(this.threads[0]);
      if (!this.threads[0].isRead && !this.threads[0].lastMessageIsMine) {
        console.log("Inside");
        this.messageService.markThreadAsRead(this.threads[0].userTwoId).subscribe();
      }
      this.firstTime = false;
    });
  }

  loadComponent(id: number){
    this.messageService.changeCurrentMessageThread(id);
    if (!this.firstTime) {
      this.messageService.markThreadAsRead(id).subscribe();
      this.messageService.getUserThreads(1).subscribe( data => {
        this.threads = data;
      });
    }
    console.log(this.threads);
  }

  onScroll() {
    if (this.notscrolly && this.notEmptyPost) {
      this.spinner.show();
      this.notscrolly = false;
      this.loadNextData();
   }
  }

  loadNextData() {
    this.currentPage += 1;
    console.log(this.currentPage);
    this.messageService.getUserThreads(this.currentPage)
    .subscribe( (data) => {
      console.log(data);
      if (data.length === 0 ) {
        this.notEmptyPost =  false;
      }

      for (const item of data) {
        this.threads.push(item);
      }

      this.spinner.hide();
      this.notscrolly = true;
    });
  }
}
