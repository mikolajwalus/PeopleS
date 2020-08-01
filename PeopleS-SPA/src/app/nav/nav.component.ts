import { Component, OnInit } from '@angular/core';
import { map } from 'rxjs/operators';
import { AuthService } from '../_services/auth.service';
import { BsDropdownConfig } from 'ngx-bootstrap/dropdown';
import { Router } from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css'],
  providers: [{ provide: BsDropdownConfig, useValue: { isAnimated: true, autoClose: true } }]
})
export class NavComponent implements OnInit {
  model: any = {};
  user: any = {};
  logged: boolean;

  constructor(private authService: AuthService,
              private router: Router,
              private alertifyService: AlertifyService) { }

  ngOnInit() {
    this.authService.isLoggedIn$.subscribe(status => this.logged = status);
  }

  login() {
    console.log(this.model);
    this.authService.login(this.model)
    .pipe( map( (response: any) => {
      this.user = JSON.parse(JSON.stringify(response.user));
    } )
    )
    .subscribe( next => {
      this.authService.succesfullLoggedIn();
      this.router.navigate(['home']);
      this.alertifyService.success('Logged in successfully');
    },
    error => {
      this.alertifyService.error('Incorrect login or password');
    });
  }

  logout() {
    this.authService.logout();
    this.user = {};
    this.model = {};
    this.alertifyService.success('Succesfully logged out');
  }

  toProfile() {
    this.router.navigate([this.authService.getToken().nameid]);
  }

  toSearch() {
    this.router.navigate(['search']);
  }
}
