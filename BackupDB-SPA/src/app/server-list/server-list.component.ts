import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Database } from '../_models/database';
import { Server } from '../_models/server';
import { AlertifyService } from '../_services/alertify.service';
import { ServerService } from '../_services/server.service';

@Component({
  selector: 'app-server-list',
  templateUrl: './server-list.component.html',
  styleUrls: ['./server-list.component.css']
})
export class ServerListComponent implements OnInit {
  servers: Server[];
  databases:any;
  model: any = {};

  constructor(private userService: ServerService, private alertify: AlertifyService,
    private route: ActivatedRoute ,private http: HttpClient) { }

  ngOnInit() {
    // this.route.data.subscribe(data => {
    //   this.servers = data['server'];
    // });

    this.http.get<Server[]>('http://localhost:5051/api/BackUp/servers').subscribe(response => {
      this.servers = response;
      console.log('in on init of server-list');
    }, error => {
      console.log(error);
    });
  }

  log(event: boolean, server: Server) {
    console.log(`Accordion has been ${event ? 'opened' : 'closed'}`);
    if(event)
    {
      this.http.post('http://localhost:5051/api/Backup/DataBases',server).subscribe(response => {
        this.databases = response;
        console.log(this.databases);
      }, error => {
        console.log(error);
      });
    }
    
  }

}
