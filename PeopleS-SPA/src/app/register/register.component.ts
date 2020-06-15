import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  loginModel: any = {};
  registerModel: any = {};
  registerForm: FormGroup;
  user: any = {};
  logged: boolean;
  toRegister = false;

  constructor(private auth: AuthService, private fb: FormBuilder) { }

  ngOnInit() {
    this.auth.isLoggedIn$.subscribe(status => this.logged = status);
    this.createRegisterForm();
  }

  createRegisterForm() {
    this.registerForm = this.fb.group( {
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(7), Validators.maxLength(15)] ],
      confirmPassword: ['', Validators.required]
    }, {validator: this.passwordMatchValidator}
    );
  }

  passwordMatchValidator(g: FormGroup) {
    return g.get('password').value === g.get('confirmPassword').value ? null : { mismatch : true};
  }

  register() {
    this.registerModel = Object.assign(this.registerForm.value);
    this.auth.register(this.registerModel).subscribe();
  }

}
