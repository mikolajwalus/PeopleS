import { Injectable } from '@angular/core';
import * as alertify from '../../../node_modules/alertifyjs/build/alertify.js';

@Injectable({
  providedIn: 'root'
})
export class AlertifyService {

  constructor() { }

  success(message: string) {
    alertify.success(message);
 }

 error(message: string) {
   alertify.error(message);
 }

 warning(message: string) {
   alertify.warning(message);
 }

 message(message: string) {
   alertify.message(message);
 }
}
