import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { UserSearch } from '../_models/user-search';
import { NgxSpinnerService } from 'ngx-spinner';
import { UserService } from '../_services/user.service';
import { AuthService } from '../_services/auth.service';

@Component({
  selector: 'app-friends',
  templateUrl: './friends.component.html',
  styleUrls: ['./friends.component.css']
})
export class FriendsComponent implements OnInit {
  radioModel = 'Friends';
  users: UserSearch[];

  notEmptyPost = true;
  notscrolly = true;

  currentPage = 1;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private spinner: NgxSpinnerService,
    private userService: UserService,
    private authService: AuthService,) { }

  ngOnInit(): void {

    this.route.data.subscribe( data => {
      console.log(data.friender);
      this.users = data.friender;
    });

  }

  onScroll() {
    if (this.notscrolly && this.notEmptyPost) {
      this.spinner.show();
      this.notscrolly = false;
      this.loadNextData();
   }
  }

  loadNextData() {
    this.currentPage += 1;
    console.log(this.currentPage);
    if (this.radioModel === 'Friends') {
      this.userService.getFriends(this.authService.getToken().nameid , this.currentPage)
      .subscribe( (data: UserSearch[]) => {
        if (data.length === 0 ) {
          this.notEmptyPost =  false;
        }

        for (const item of data) {
          this.users.push(item);
        }

        this.spinner.hide();
        this.notscrolly = true;
      });
    }
    
  }
  
  toProfile(id: number) {
    this.router.navigate([id]);
  }

  loadFriends() {
    this.userService.getFriends(this.authService.getToken().nameid, 1).subscribe( data => {
      this.users = data;
    });
  }

  loadSent() {
    this.userService.getSentInvitation(this.authService.getToken().nameid, 1).subscribe( data => {
      this.users = data;
    });
  }

  loadRecieved() {
    this.userService.getInvitations(this.authService.getToken().nameid, 1).subscribe( data => {
      this.users = data;
    });
  }

  addFriend(user: UserSearch) {
    const index = this.users.indexOf(user, 0);
    if (index > -1) {
      this.users.splice(index, 1);
    }

    this.userService.addFriend(user.id).subscribe();
  }
}
