import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Database } from '../_models/database';
import { Server } from '../_models/server';

const httpOptions = {
  headers: new HttpHeaders({
    'Authorization': 'Bearer ' + localStorage.getItem('token')
  })
};

@Injectable({
  providedIn: 'root'
})
export class ServerService {
  baseUrl = environment.apiUrl;
  result : any

constructor(private http: HttpClient) { }

getServers() {
  this.http.get<Server[]>(this.baseUrl + 'BackUp/servers',httpOptions).subscribe(res => {this.result = res}, error => {  console.log(error) });
  //console.log(this.result);
  return this.result;
}

getDatabases(name): Observable<Database[]> {
  return this.http.get<Database[]>(this.baseUrl + 'BackUp/databases/' + name, httpOptions);
}
}
