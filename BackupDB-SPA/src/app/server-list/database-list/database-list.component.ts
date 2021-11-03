import { HttpClient } from '@angular/common/http';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Database } from 'src/app/_models/database';
import { Server } from 'src/app/_models/server';
import { NotificationService } from 'src/app/_services/notification.service';

@Component({
  selector: 'app-database-list',
  templateUrl: './database-list.component.html',
  styleUrls: ['./database-list.component.css']
})
export class DatabaseListComponent implements OnInit {
  databasesBackups : Database[] = <Database[]>{};
  databases: Database[] = <Database[]>{};
  @Input() server : Server = <Server>{};
  @Output() emitPassDBIncludeInBackUpEvent : EventEmitter<any> = new EventEmitter();
  constructor(private http: HttpClient, private notify: NotificationService,) { }

  ngOnInit() {
    this.http
    .post<Database[]>('http://localhost:5051/api/Backup/DataBases', this.server)
    .subscribe(
      (response) => {
        this.databasesBackups = response;
         this.databases = this.databasesBackups.filter(x =>x.rank === 1);
      },
      (error) => {
        console.log(error);
        this.notify.error(error);
      }
    );
  }

  PassDBIncludeInBackUpList(database:any)
  {
    console.log("emit->" );
    console.log( database);
    this.emitPassDBIncludeInBackUpEvent.emit( { data: database.data , res:200});
  }

}
