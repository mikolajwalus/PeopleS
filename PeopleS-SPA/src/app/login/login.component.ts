import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { AuthService } from '../_services/auth.service';
import { map } from 'rxjs/operators';
import { Router } from '@angular/router';
import { ThrowStmt } from '@angular/compiler';
import { AlertifyService } from '../_services/alertify.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  loginModel: any = {};
  user: any = {};
  logged: boolean;

  constructor(private authService: AuthService,
              private fb: FormBuilder,
              private router: Router,
              private alertifyService: AlertifyService) { }

  ngOnInit() {
    this.authService.isLoggedIn$.subscribe(status => this.logged = status);
  }

  login() {
    this.authService.login(this.loginModel)
    .pipe( map( (response: any) => {
      this.user = JSON.parse(JSON.stringify(response.user));
    } )
    )
    .subscribe( next => {
      this.authService.succesfullLoggedIn();
      this.router.navigate(['home']);
      this.alertifyService.success('Logged in successfully');
    }, error => {
      this.alertifyService.error('Incorrect login or password');
    });
  }

}
