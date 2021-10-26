import { HttpClient } from '@angular/common/http';
import { Component, Input, OnInit } from '@angular/core';
import { Database } from 'src/app/_models/database';
import { DBBackUpProcessInfo } from 'src/app/_models/dbBackUpProcessInfo';
import { Server } from 'src/app/_models/server';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  selector: 'app-database-list',
  templateUrl: './database-list.component.html',
  styleUrls: ['./database-list.component.css']
})
export class DatabaseListComponent implements OnInit {
  @Input() database: Database;
  @Input() server :Server;
  dbBackProcessInfo : DBBackUpProcessInfo = { serverName: '', DBName:''};
  result :any;
  constructor( private alertify: AlertifyService,private http: HttpClient) { }

  ngOnInit() {
  }

  backup(){
    this.dbBackProcessInfo.DBName = this.database.database_name;
    this.dbBackProcessInfo.serverName = this.server.serverName;
    this.alertify.warning('backup in progress');
    this.http.post('http://localhost:5051/api/Backup/Process',this.dbBackProcessInfo).subscribe(response => {
      this.result = response;
      console.log('result of Process:'+this.result);
    }, error => {
      console.log(error);
    } );
    this.alertify.success('backup successfully');
  }

}
