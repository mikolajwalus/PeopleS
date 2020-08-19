import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgxSpinnerService } from 'ngx-spinner';
import { UserService } from '../_services/user.service';
import { AuthService } from '../_services/auth.service';
import { Post } from '../_models/post';
import { FileUploader } from 'ng2-file-upload';
import { environment } from 'src/environments/environment';
@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  visible: boolean[] = [true, false, false];
  allpost: Post[];
  notEmptyPost = true;
  notscrolly = true;
  currentPage: number;
  content = '';
  emptyMessage = false;
  isMe = false;
  uploader: FileUploader;
  baseUrl = environment.apiUrl;
  hasBaseDropZoneOver: boolean;

 constructor(
    private route: ActivatedRoute,
    private spinner: NgxSpinnerService,
    private userService: UserService,
    private authService: AuthService,
  ) {
}

  ngOnInit() {
    this.route.data.pipe(
    ).subscribe( data => {
      this.allpost = data.homer;
    });
    this.currentPage = 1;
    this.initializeUploader();
}

  initializeUploader() {
  this.uploader = new FileUploader({
    url: this.baseUrl + 'users/addPost',
    authToken: 'Bearer ' + localStorage.getItem('token'),
    isHTML5: true,
    allowedFileType: ['image'],
    removeAfterUpload: true,
    autoUpload: false,
    maxFileSize: 10 * 1024 * 1024,
  });

  this.uploader.onBuildItemForm = (fileItem: any, form: any) => {
    form.append('text', this.content);
    form.append('userId', this.authService.getToken().nameid.toString());
   };

  this.uploader.onAfterAddingFile = (file) => {file.withCredentials = false; };

  this.uploader.onSuccessItem = (item, response, status, headers) => {
    this.content = '';
  };

}

fileOverBase(e: any): void {
  this.hasBaseDropZoneOver = e;
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

  this.userService.getUserDashboard( this.authService.getToken().nameid, this.currentPage)
  .subscribe( (data) => {

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
