import { Component, OnInit } from '@angular/core';
import { Value } from '../_models/value';
import { ValuesService } from '../_services/values.service';

@Component({
  selector: 'app-value',
  templateUrl: './value.component.html',
  styleUrls: ['./value.component.css']
})
export class ValueComponent implements OnInit {
  values: Value[];
  title = 'PeopleS';

  constructor(private valuesService: ValuesService) { }

  ngOnInit() {
    this.valuesService.getValues().subscribe( response => {
      this.values = response;
    });
  }
}
