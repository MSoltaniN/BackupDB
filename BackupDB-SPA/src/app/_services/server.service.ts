import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Database } from '../_models/database';
import { Server } from '../_models/server';
import { NotificationService } from './notification.service';

const httpOptions = {
  headers: new HttpHeaders({
    Authorization: 'Bearer ' + localStorage.getItem('token'),
  }),
};

@Injectable({
  providedIn: 'root',
})
export class ServerService {
  baseUrl = environment.apiUrl + 'api/BackUp/';
  result: any;

  constructor(private http: HttpClient, private notify: NotificationService) {}

  getServers() {
    this.http.get<Server[]>(this.baseUrl + 'servers', httpOptions).subscribe(
      (res) => {
        this.result = res;
      },
      (error) => {
        console.log(error);
        this.notify.error(error);
      }
    );
    //console.log(this.result);
    return this.result;
  }

  // getDatabases(server: Server): Observable<Database[]> {
  //   return this.http
  //     .get<Database[]>(this.baseUrl + 'databases' + server, httpOptions)
  //     .subscribe(
  //       (res) => {
  //         this.result = res;
  //       },
  //       (error) => {
  //         console.log(error);
  //         this.notify.error(error);
  //       }
  //     );
  // }
}
