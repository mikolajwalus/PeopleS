import { Component, OnInit } from '@angular/core';
import { UserService } from '../_services/user.service';
import { ActivatedRoute } from '@angular/router';
import { User } from '../_models/User';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { FileUploader } from 'ng2-file-upload';
import { DatePipe } from '@angular/common';
import { environment } from 'src/environments/environment';


@Component({
  selector: 'app-user-editor',
  templateUrl: './user-editor.component.html',
  styleUrls: ['./user-editor.component.css'],
  providers: [ DatePipe ]
})
export class UserEditorComponent implements OnInit {
  bsConfig = Object.assign({}, { containerClass: 'theme-dark-blue', useUtc: true });
  bsInlineValue = new Date();
  bsInlineRangeValue: Date[];
  profileModel: any = {};
  emailForm: FormGroup;
  passwordForm: FormGroup;
  dateForm: FormGroup;
  profileForm: FormGroup;
  maxDate = new Date();
  user: User;
  uploader: FileUploader;
  baseUrl = environment.apiUrl;
  hasBaseDropZoneOver: boolean;

  constructor(  private userService: UserService,
                private route: ActivatedRoute,
                private fb: FormBuilder,
                private authService: AuthService,
                private alertify: AlertifyService,
                private datePipe: DatePipe) {
    this.maxDate.setDate(this.maxDate.getDate() + 7);
    this.bsInlineRangeValue = [this.bsInlineValue, this.maxDate];
  }

  ngOnInit(): void {
    this.route.data.subscribe( data => {
      this.user = data.user;
    });
    this.createEmailForm();
    this.createPasswordForm();
    this.createDateForm();
    this.createProfileForm();
    this.initializeUploader();
  }

  public fileOverBase(e: any): void {
    this.hasBaseDropZoneOver = e;
  }

  initializeUploader() {
    this.uploader = new FileUploader({
      url: this.baseUrl + 'users/' + this.authService.getToken().nameid + '/changePhoto',
      authToken: 'Bearer ' + localStorage.getItem('token'),
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024,
    });

    this.uploader.onAfterAddingFile = (file) => {file.withCredentials = false; };

    this.uploader.onSuccessItem = (item, response, status, headers) => {
      this.userService.getUser(this.authService.getToken().nameid). subscribe( data => {
        this.user = data;
      })
    };
  }

  createEmailForm() {
    this.emailForm = this.fb.group( {
      email: [this.user.email, [Validators.required, Validators.email]],
    });
  }

  createPasswordForm() {
    this.passwordForm = this.fb.group( {
      oldPassword: ['', [Validators.required, Validators.minLength(7), Validators.maxLength(15)] ],
      password: ['', [Validators.required, Validators.minLength(7), Validators.maxLength(15)] ],
      confirmPassword: ['', Validators.required],
    }, {validator: this.passwordMatchValidator}
    );
  }

  createDateForm() {
    this.dateForm = this.fb.group( {
      dateOfBirth: [this.datePipe.transform(this.user.dateOfBirth, 'short')],
    });
  }

  createProfileForm() {
    this.profileForm = this.fb.group( {
      name: [this.user.name, Validators.required],
      surname: [this.user.surname],
      aboutMe: [this.user.aboutMe],
      city: [this.user.city],
      country: [this.user.country],
      company: [this.user.company],
      school: [this.user.school],
      myInterests: [this.user.myInterests]
    });
  }

  saveChanges(){
    this.userService.updateUser(this.authService.getToken().nameid, this.profileForm.value)
      .subscribe( next => {
        this.alertify.success('Profile successfully updated');
        this.updateOtherInformations();
        this.profileForm.reset(this.user);
      }, error => {
        this.alertify.error('Error during updating profile');
      });
  }

  changePassword() {
    this.userService.changePassword(this.authService.getToken().nameid, this.passwordForm.value)
      .subscribe( next => {
        this.alertify.success('Password succesfully changed!');
        this.passwordForm.reset();
      }, error => {
        this.alertify.error('Error during changing password!');
      });
  }

  changeDateOfBirth() {
    const dateString = JSON.stringify(this.dateForm.value);
    const obj = JSON.parse(dateString);
    this.userService.changeDate(this.authService.getToken().nameid, obj)
    .subscribe( next => {
      this.alertify.success('Date of birth succesfully changed!');
      console.log('User: ');
      console.log(this.user.dateOfBirth);
      this.updateDateOfBirth();
      console.log('User: ');
      console.log(this.user.dateOfBirth);
      console.log('Form: ');
      console.log(this.dateForm.value.dateOfBirth);
      console.log('Z: ');
      console.log(this.datePipe.transform(this.user.dateOfBirth, 'short', 'UTC+8'));
      this.dateForm.reset({
        dateOfBirth: this.datePipe.transform(this.user.dateOfBirth, 'short', 'UTC+8')
      });
    }, error => {
      this.alertify.error('Error during changing date of birth!');
    });
  }

  updateDateOfBirth() {
    this.user.dateOfBirth = this.dateForm.value.dateOfBirth;
  }

  updateOtherInformations() {
    this.user.name = this.profileForm.value.name;
    this.user.surname = this.profileForm.value.surname;
    this.user.school = this.profileForm.value.school;
    this.user.aboutMe = this.profileForm.value.aboutMe;
    this.user.city = this.profileForm.value.city;
    this.user.country = this.profileForm.value.country;
    this.user.company = this.profileForm.value.company;
    this.user.myInterests = this.profileForm.value.myInterests;
  }

  toMain() {
    document.getElementById('main').scrollIntoView({behavior: 'smooth', block: 'center', inline: 'nearest'});
  }

  toChangePassword() {
    document.getElementById('password').scrollIntoView({behavior: 'smooth', block: 'center', inline: 'nearest'});
  }

  toChangeDate() {
    document.getElementById('date').scrollIntoView({behavior: 'smooth', block: 'center', inline: 'nearest'});
  }

  toMoreInformations() {
    document.getElementById('moreInfo').scrollIntoView({behavior: 'smooth', block: 'nearest', inline: 'nearest'});
  }

  passwordMatchValidator(g: FormGroup) {
    return g.get('password').value === g.get('confirmPassword').value ? null : { mismatch : true};
  }
}
