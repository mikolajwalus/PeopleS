import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {JwtHelperService, JwtModule} from '@auth0/angular-jwt';
import { environment } from 'src/environments/environment';
import { map } from 'rxjs/operators';
import { BehaviorSubject } from 'rxjs';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
baseUrl = environment.apiUrl;
token: any;
jwtHelper = new JwtHelperService();
private loggedIn = new BehaviorSubject<boolean>(this.isLoggedIn());
public isLoggedIn$ = this.loggedIn.asObservable();

constructor(private http: HttpClient, private router: Router) { }

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
  this.router.navigate(['login']);
}

succesfullLoggedIn() {
  this.loggedIn.next(true);
}

register(model: any) {
  return this.http.post(this.baseUrl + 'auth/register', model);
}

isLoggedIn() {
  const token = localStorage.getItem('token');
  return !this.jwtHelper.isTokenExpired( token );
}

}
