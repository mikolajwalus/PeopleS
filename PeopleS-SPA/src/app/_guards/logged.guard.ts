import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, Router } from '@angular/router';
import { AuthService } from '../_services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class LoggedGuard implements CanActivate {

  constructor(private authService: AuthService, private router: Router) { }

  canActivate(): boolean {

    console.log('Logged guard:' + this.authService.isLoggedIn() );

    if ( !this.authService.isLoggedIn() ) {
      return true;
    }

    this.router.navigate(['home']);

    return false;
  }
}
