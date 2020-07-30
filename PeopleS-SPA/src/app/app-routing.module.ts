import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { HomeComponent } from './home/home.component';
import { AuthGuard } from './_guards/AuthGuard';
import { LoggedGuard } from './_guards/logged.guard';
import { UserEditorComponent } from './user-editor/user-editor.component';
import { UserEditor } from './_resolvers/user-editor.resolver';
import { UserProfileComponent } from './user-profile/user-profile.component';
import { UserProfile } from './_resolvers/user-profile.resolver';
import { UserSearchComponent } from './user-search/user-search.component';
import { UserSearcher } from './_resolvers/user-search.resolver';


const routes: Routes = [
  { path: 'login', component: LoginComponent, canActivate: [LoggedGuard] },
  { path: 'register', component: RegisterComponent, canActivate: [LoggedGuard] },
  { path: 'home', component: HomeComponent, canActivate: [AuthGuard] },
  { path: 'edit-profile', component: UserEditorComponent, resolve: {user: UserEditor} , canActivate: [AuthGuard] },
  { path: 'search', component: UserSearchComponent, canActivate: [AuthGuard]},
  { path: 'search/:q', component: UserSearchComponent, resolve: {userSearcher: UserSearcher}, canActivate: [AuthGuard]},
  { path: ':id', component: UserProfileComponent, resolve: {userprofile: UserProfile}, canActivate: [AuthGuard] },
  { path: '',   redirectTo: '/login', pathMatch: 'full' },
  { path: '**', redirectTo: '/login', pathMatch: 'full' },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
