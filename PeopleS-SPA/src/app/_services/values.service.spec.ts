/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { ValuesService } from './values.service';

describe('Service: Values', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [ValuesService]
    });
  });

  it('should ...', inject([ValuesService], (service: ValuesService) => {
    expect(service).toBeTruthy();
  }));
});
