import { Component, OnInit } from '@angular/core';
import { map } from 'rxjs/operators';
import { AuthService } from '../_services/auth.service';
import { BsDropdownConfig } from 'ngx-bootstrap/dropdown';

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

  constructor(private auth: AuthService) { }

  ngOnInit() {
    this.auth.isLoggedIn$.subscribe(status => this.logged = status);
  }

  login() {
    console.log(this.model);
    this.auth.login(this.model)
    .pipe( map( (response: any) => {
      this.user = JSON.parse(JSON.stringify(response.user));
    } )
    )
    .subscribe( (response) => {
      this.auth.succesfullLoggedIn();
    });
  }

  logout() {
    this.auth.logout();
    this.user = {};
    this.model = {};
  }
}
