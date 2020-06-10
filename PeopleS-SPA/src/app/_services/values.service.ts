import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable} from 'rxjs';
import { Value } from '../_models/value';

@Injectable({
  providedIn: 'root'
})
export class ValuesService {

constructor(private http: HttpClient) { }
configUrl = 'http://localhost:5000/';

getValues(): Observable<Value[]> {
  return this.http.get<Value[]>(this.configUrl + 'Values');
}

getValue(id: number): Observable<Value> {
  return this.http.get<Value>(this.configUrl + 'Values/' + id);
}

}
