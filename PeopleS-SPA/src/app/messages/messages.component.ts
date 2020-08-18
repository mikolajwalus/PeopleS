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

  ngOnInit(): void {
    this.route.data.subscribe( data => {
      this.threads = data.messanger;
      this.loadComponent(this.threads[0].userTwoId);
    });
  }

  loadComponent(id: number){
    this.messageService.ChangeCurrentMessageThread(id);
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
    this.messageService.GetUserThreads(this.currentPage)
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
