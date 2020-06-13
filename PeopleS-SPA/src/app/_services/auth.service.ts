import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { map } from 'rxjs/operators';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
baseUrl = environment.apiUrl;
token: any;
private loggedIn = new BehaviorSubject<boolean>(false);
public isLoggedIn$ = this.loggedIn.asObservable();

constructor(private http: HttpClient) { }

login(model: any) {
  return this.http.post( this.baseUrl + 'auth/login', model )
    .pipe(
      map( (response: any) => {
        localStorage.setItem('token', response.token);
        return response;
      })
    );
}

logout() {
  localStorage.removeItem('token');
  this.loggedIn.next(false);
}

succesfullLoggedIn() {
  this.loggedIn.next(true);
}

}
