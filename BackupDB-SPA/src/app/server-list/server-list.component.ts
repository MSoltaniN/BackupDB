import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
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

  constructor(private userService: ServerService, private alertify: AlertifyService,
    private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.servers = data['servers'];
    });
    console.log("server-list.component" );
    console.log( this.servers );
  }

}
