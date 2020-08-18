import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { Injectable } from '@angular/core';
import { UserService } from '../_services/user.service';
import { catchError } from 'rxjs/operators';
import { AlertifyService } from '../_services/alertify.service';
import { of } from 'rxjs';
import { AuthService } from '../_services/auth.service';


@Injectable({
    providedIn: 'root'
})


export class Homer implements Resolve<any> {

    constructor(    private userService: UserService,
                    private authService: AuthService,
                    private alertify: AlertifyService,
                    private router: Router) { }

  resolve( router: ActivatedRouteSnapshot ) {
    return this.userService.getUserDashboard(this.authService.getToken().nameid, 1)
        .pipe(
            catchError( error => {
            this.alertify.error(error.error);
            this.router.navigate(['home']);
            return of(null);
        })
    );
  }
}