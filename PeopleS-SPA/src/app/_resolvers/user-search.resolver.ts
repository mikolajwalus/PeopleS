import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { Injectable } from '@angular/core';
import { UserService } from '../_services/user.service';
import { AuthGuard } from '../_guards/AuthGuard';
import { AuthService } from '../_services/auth.service';
import { catchError, map } from 'rxjs/operators';
import { AlertifyService } from '../_services/alertify.service';
import { of } from 'rxjs';


@Injectable({
    providedIn: 'root'
})


export class UserSearcher implements Resolve<any> {

    constructor(    private userService: UserService,
                    private authService: AuthService,
                    private alertify: AlertifyService,
                    private router: Router) { }

  resolve( router: ActivatedRouteSnapshot ) {
    return this.userService.searchUsers(router.params['q'], 1)
        .pipe(
            catchError( error => {
            this.alertify.error(error.error);
            this.router.navigate(['home']);
            return of(null);
        })
    );
  }
}