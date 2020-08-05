import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { JwtModule } from '@auth0/angular-jwt';
import { InfiniteScrollModule } from 'ngx-infinite-scroll';
import { NgxSpinnerModule } from 'ngx-spinner';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { ValuesService } from './_services/values.service';
import { ValueComponent } from './value/value.component';
import { NavComponent } from './nav/nav.component';
import { HomeComponent } from './home/home.component';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { AuthGuard } from './_guards/AuthGuard';
import { LoggedGuard } from './_guards/logged.guard';
import { AlertifyService } from './_services/alertify.service';
import { UserEditorComponent } from './user-editor/user-editor.component';
import { UserService } from './_services/user.service';
import { AuthService } from './_services/auth.service';
import { UserProfileComponent } from './user-profile/user-profile.component';
import { UserSearchComponent } from './user-search/user-search.component';
import { MessagesComponent } from './messages/messages.component';

export function tokenGetter() {
   return localStorage.getItem('token');
}

@NgModule({
   declarations: [
      AppComponent,
      ValueComponent,
      NavComponent,
      HomeComponent,
      LoginComponent,
      RegisterComponent,
      UserEditorComponent,
      UserProfileComponent,
      UserSearchComponent,
      MessagesComponent
   ],
   imports: [
      BrowserModule,
      AppRoutingModule,
      FormsModule,
      HttpClientModule,
      BrowserAnimationsModule,
      BsDatepickerModule.forRoot(),
      BsDropdownModule.forRoot(),
      ReactiveFormsModule,
      JwtModule.forRoot({
         config: {
            tokenGetter: tokenGetter,
            whitelistedDomains: ['localhost:5000'],
            blacklistedRoutes: ['localhost:5000/api/auth']
         }
      }),
      InfiniteScrollModule,
      NgxSpinnerModule,
   ],
   providers: [
      AuthService,
      ValuesService,
      AlertifyService,
      UserService,
      AuthGuard,
      LoggedGuard
   ],
   bootstrap: [
      AppComponent
   ]
})
export class AppModule { }
