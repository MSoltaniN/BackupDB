import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Database } from '../_models/database';
import { Server } from '../_models/server';
import { NotificationService } from '../_services/notification.service';
import { ServerService } from '../_services/server.service';
import { Pipe } from '@angular/core';
import { BackUpComponent } from '../shared/backUp/backUp.component';
import { BsModalService } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-server-list',
  templateUrl: './server-list.component.html',
  styleUrls: ['./server-list.component.css'],
})
export class ServerListComponent implements OnInit {
  servers: Server[] = <Server[]>{};
  databases: Database[] = <Database[]>{};
  model: any = {};
  groupBackUpdisabled = true;
   backUpList: Database[] = [];

  constructor(
    private userService: ServerService,
    private notify: NotificationService,
    private route: ActivatedRoute,
    private http: HttpClient,
    private modalService: BsModalService
  ) {}

  ngOnInit() {
    // this.route.data.subscribe(data => {
    //   this.servers = data['server'];
    // });
    console.log('in on init of server-list');
    this.http
      .get<Server[]>('http://localhost:5051/api/BackUp/servers')
      .subscribe(
        (response) => {
          this.servers = response;
          console.log(this.servers);
          // this.snackBar.open("massege","action")
        },
        (error) => {
          console.log(error);
          this.notify.error(error);
        }
      );

 }

  log(event: boolean, server: Server) {
    console.log(`Accordion has been ${event ? 'opened' : 'closed'}`);
    if (event) {
      this.http
        .post<Database[]>('http://localhost:5051/api/Backup/DataBases', server)
        .subscribe(
          (response) => {
            this.databases = response;
            console.log(this.databases);
          },
          (error) => {
            console.log(error);
            this.notify.error(error);
          }
        );
    }
    else
      this.RemoveDBIncludeInBackUpList();
  }

  UpdateDBIncludeInBackUpList(database:any) {
  
    if(database.data.include_backup_process === true)
     this.backUpList.push(database.data);
     else
     {
      var count = this.backUpList.indexOf(database.data);
      this.backUpList.splice(count,1)
     }
     

    if (this.backUpList.filter((x) => (x.include_backup_process = true)).length >  0 )
      this.groupBackUpdisabled = false;
    else
      this.groupBackUpdisabled = true;


    console.log(this.backUpList.filter((x) => (x.include_backup_process = true)));
  }

  RemoveDBIncludeInBackUpList()
  {
      this.backUpList.splice(0,this.backUpList.length);
      this.groupBackUpdisabled = true;
  }

  OnClickGroupBackUp()
  {
    const initialState = { databases: this.backUpList.filter((x) => (x.include_backup_process = true)) };
    this.modalService.show(BackUpComponent , { initialState });
    
    
  }


}
