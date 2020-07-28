import { Resolve, Router, ActivatedRoute, ActivatedRouteSnapshot } from '@angular/router';
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

  resolve( router: ActivatedRouteSnapshot ) {
    console.log( router.params['id']);
    return this.userService.getUserPosts( router.params['id'] , 1 )
        .pipe(
            catchError( error => {
            this.alertify.error(error.error);
            this.router.navigate(['home']);
            return of(null);
        })
    );
  }
}
