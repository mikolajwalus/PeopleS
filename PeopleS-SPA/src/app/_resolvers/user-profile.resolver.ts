import { Resolve, Router } from '@angular/router';
import { Injectable } from '@angular/core';
import { UserService } from '../_services/user.service';
import { AuthService } from '../_services/auth.service';
import { catchError } from 'rxjs/operators';
import { AlertifyService } from '../_services/alertify.service';
import { of } from 'rxjs';


@Injectable({
    providedIn: 'root'
})


export class UserProfile implements Resolve<any> {

    constructor(    private userService: UserService,
                    private authService: AuthService,
                    private alertify: AlertifyService,
                    private router: Router) { }

  resolve() {
    console.log(this.authService.getToken());
    return this.userService.getUserPosts( this.authService.getToken().nameid, 1 )
        .pipe(
            catchError( error => {
            this.alertify.error('Problem during data retrieving');
            this.router.navigate(['home']);
            return of(null);
        })
    );
  }
}
