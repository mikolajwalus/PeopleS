import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { map } from 'rxjs/operators';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  loginModel: any = {};
  registerForm: FormGroup;
  user: any = {};
  logged: boolean;
  toRegister = false;

  constructor(private auth: AuthService, private fb: FormBuilder) { }

  ngOnInit() {
    this.auth.isLoggedIn$.subscribe(status => this.logged = status);
    this.createRegisterForm();
  }

  login() {
    this.auth.login(this.loginModel)
    .pipe( map( (response: any) => {
      this.user = JSON.parse(JSON.stringify(response.user));
    } )
    )
    .subscribe( (response) => {
      console.log(this.user);
      this.auth.succesfullLoggedIn();
    });
  }

  changeContent() {
    this.toRegister = !this.toRegister;
  }

  createRegisterForm() {
    this.registerForm = this.fb.group( {
      email: ['', Validators.required, Validators.email],
      password: ['',[Validators.required, Validators.minLength(7), Validators.maxLength(15)]],
      confirmPassword: ['', Validators.required]
    }, {validator: this.passwordMatchValidator}
    );
  }

  passwordMatchValidator(g: FormGroup) {
    return g.get('password').value === g.get('confirmPassword').value ? null : { mismatch : true};
  }
}
