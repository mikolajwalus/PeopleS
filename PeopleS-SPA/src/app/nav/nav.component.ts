import { Component, OnInit } from '@angular/core';
import { Value } from '../_models/value';
import { ValuesService } from '../_services/values.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  constructor() { }

  ngOnInit() {

  }
}
