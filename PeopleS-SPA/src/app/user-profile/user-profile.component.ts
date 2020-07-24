import { Component, OnInit } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';
import { ActivatedRoute } from '@angular/router';
import { UserService } from '../_services/user.service';
import { AuthService } from '../_services/auth.service';
import { Post } from 'src/app/_models/post';

@Component({
  selector: 'app-user-profile',
  templateUrl: './user-profile.component.html',
  styleUrls: ['./user-profile.component.css']
})
export class UserProfileComponent implements OnInit {  constructor(
  private route: ActivatedRoute,
  private spinner: NgxSpinnerService,
  private userService: UserService,
  private authService: AuthService
  ) {
}
allpost: Post[];
notEmptyPost = true;
notscrolly = true;
currentPage: number;
  ngOnInit() {
    this.route.data.pipe(
    ).subscribe( data => {
      this.allpost = data.userprofile;
  });
  this.currentPage = 1;
}

onScroll() {
  if (this.notscrolly && this.notEmptyPost) {
    this.spinner.show();
    this.notscrolly = false;
    this.loadNextPost();
 }
}

loadNextPost() {
  this.currentPage += 1;

  this.userService.getUserPosts( this.authService.getToken().nameid, this.currentPage).subscribe( (data: Post[]) => {

    if (data.length === 0 ) {
      this.notEmptyPost =  false;
    }

    console.log(this.allpost);

    for (const item of data) {
      this.allpost.push(item);
    }

    console.log(this.allpost);

    this.spinner.hide();
    this.notscrolly = true;
  });
}

}