import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Component, Input, OnInit, TemplateRef ,EventEmitter, Output } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { BackUpComponent } from 'src/app/shared/backUp/backUp.component';
import { PathLocatorComponent } from 'src/app/shared/pathLocator/pathLocator.component';
import { Database } from 'src/app/_models/database';
import { DBBackUpProcessInfo } from 'src/app/_models/dbBackUpProcessInfo';
import { Server } from 'src/app/_models/server';
import { NotificationService } from 'src/app/_services/notification.service';
import { environment } from 'src/environments/environment';

const httpOptions = {
  headers: new HttpHeaders({
    Authorization: 'Bearer ' + localStorage.getItem('token'),
  }),
};

@Component({
  selector: 'app-database-card',
  templateUrl: './database-card.component.html',
  styleUrls: ['./database-card.component.css']
})
export class DatabaseCardComponent implements OnInit {
  @Input() databasesBackups : Database[] = <Database[]>{};
  @Input() database: Database = <Database>{};
  @Input() server :Server = <Server>{};
  @Output() emitDBIncludeInBackUpEvent : EventEmitter<any> = new EventEmitter();
  dbBackProcessInfo : DBBackUpProcessInfo = { serverName: '', DBName:'' , DBPath:'', UserId:''};
  DBInfomodalRef: BsModalRef =<BsModalRef>{};
  checked = false;

  
  path :string = this.dbBackProcessInfo.DBPath;
  DBPath_modalRef: BsModalRef =<BsModalRef>{};
  baseUrl = environment.apiUrl + 'api/BackUp/';

  constructor( private notify: NotificationService,private http: HttpClient ,private modalService: BsModalService) { }

  ngOnInit() {
    this.http.get<string>( this.baseUrl + 'DefaultDBPath', httpOptions).subscribe( Response =>
    this.dbBackProcessInfo.DBPath = Response,
    )

    this.databasesBackups = this.databasesBackups.filter(x => x.database_name == this.database.database_name);
  }

  openModal(template: TemplateRef<any>) {
    this.DBInfomodalRef = this.modalService.show(template);
  }

  openDBPath_Confirm_Modal()
  {
    const initialState = { databases: [this.database]  };
    this.modalService.show(BackUpComponent , { initialState });
  }

  emitCheckedEvent(){
    this.database.include_backup_process = !this.database.include_backup_process;
    console.log("on datbase card - emit ");
    console.log(this.database);
    this.emitDBIncludeInBackUpEvent.emit( { data: this.database, res:200});
  }



}
