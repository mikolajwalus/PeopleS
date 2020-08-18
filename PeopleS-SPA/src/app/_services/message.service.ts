import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { AuthService } from './auth.service';
import { environment } from 'src/environments/environment';
import { MessageThreadList } from '../_models/message-thread-list';
import { BehaviorSubject } from 'rxjs';
import { Message } from '../_models/message';
import { MessageToSend } from '../_models/messageToSend';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  baseUrl = environment.apiUrl;
  currentMessageThread: number;

  private currentThread = new BehaviorSubject<number>(null);
  public currentThread$ = this.currentThread.asObservable();

  constructor( private http: HttpClient, private authService: AuthService) { }

  ChangeCurrentMessageThread(id: number) {
    this.currentThread.next(id);
  }

  GetUserThreads(pageNumber: number, pageSize: number = 10) {
    const params = new HttpParams()
      .set( 'pageNumber', pageNumber.toString() )
      .set( 'pageSize', pageSize.toString() );

    return this.http.get<MessageThreadList[]>(
      this.baseUrl + 'users/' + this.authService.getToken().nameid + '/messages/userThreads', { params });
  }

  GetMessageThread(id: number, pageNumber: number, pageSize: number = 8) {

    const params = new HttpParams()
      .set( 'secondUserId', id.toString() )
      .set( 'pageNumber', pageNumber.toString() )
      .set( 'pageSize', pageSize.toString() );

    return this.http.get<Message[]>(
      this.baseUrl + 'users/' + this.authService.getToken().nameid + '/messages/thread', { params });
  }

  SendMessage(message: MessageToSend) {
    return this.http.post<Message>( this.baseUrl + 'users/' + this.authService.getToken().nameid + '/messages/', message );
  }
}
