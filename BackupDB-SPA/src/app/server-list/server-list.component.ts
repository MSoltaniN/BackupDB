import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Database } from '../_models/database';
import { Server } from '../_models/server';
import { NotificationService } from '../_services/notification.service';
import { ServerService } from '../_services/server.service';
import { Pipe} from '@angular/core';

@Component({
  selector: 'app-server-list',
  templateUrl: './server-list.component.html',
  styleUrls: ['./server-list.component.css']
})
export class ServerListComponent implements OnInit {
  servers: Server[] = <Server[]>{};
  databases: Database[] = <Database[]>{};
  model: any = {};
  disabled = true;

  constructor(private userService: ServerService, private notify: NotificationService,
    private route: ActivatedRoute ,private http: HttpClient ) { }

  ngOnInit() {
    // this.route.data.subscribe(data => {
    //   this.servers = data['server'];
    // });
    console.log('in on init of server-list');
    this.http.get<Server[]>('http://localhost:5051/api/BackUp/servers').subscribe(response => {
      this.servers = response;
      console.log( this.servers);
     // this.snackBar.open("massege","action")
    }, error => {
      console.log(error);
      this.notify.error(error);
    });
  }

  log(event: boolean, server: Server) {
    console.log(`Accordion has been ${event ? 'opened' : 'closed'}`);
    if(event)
    {
      this.http.post<Database[]>('http://localhost:5051/api/Backup/DataBases',server).subscribe(response => {
        this.databases =  response;
        console.log(this.databases);
      }, error => {
        console.log(error);
        this.notify.error(error);
      });
    }
    
  }

}