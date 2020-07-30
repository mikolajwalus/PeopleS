import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { UserSearch } from '../_models/user-search';
import { UserService } from '../_services/user.service';
import { NgxSpinnerService } from 'ngx-spinner';
import { AuthService } from '../_services/auth.service';

@Component({
  selector: 'app-user-search',
  templateUrl: './user-search.component.html',
  styleUrls: ['./user-search.component.css']
})
export class UserSearchComponent implements OnInit {constructor(
  private route: ActivatedRoute,
  private userService: UserService,
  private router: Router,
  private spinner: NgxSpinnerService,
  private authService: AuthService) { }

  query: string;
  queryForNextResults: string;
  currentPage = 1;
  searchResult: UserSearch[];

  notEmptyPost = true;
  notscrolly = true;

  ngOnInit() {
    this.route.data.subscribe( data => {
      this.searchResult = data.userSearcher;
    });

    this.route.params.subscribe( data => {
      this.query = data['q'];
    });

    this.queryForNextResults = this.query;
  }

  onSearch() {
    this.router.navigate(['search', this.query]);
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
    console.log(this.currentPage);
    this.userService.searchUsers(this.queryForNextResults , this.currentPage)
    .subscribe( (data: UserSearch[]) => {
      console.log('halo');
      if (data.length === 0 ) {
        this.notEmptyPost =  false;
      }

      console.log(this.searchResult);

      for (const item of data) {
        this.searchResult.push(item);
      }

      console.log(this.searchResult);

      this.spinner.hide();
      this.notscrolly = true;
    });
  }

  userCheck(id: number, status: number): number {
    if( id === this.authService.getToken().nameid ) return 5;
    return status
  }
}
