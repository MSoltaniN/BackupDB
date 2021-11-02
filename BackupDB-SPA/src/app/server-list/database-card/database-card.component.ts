import { HttpClient } from '@angular/common/http';
import { Component, Input, OnInit, TemplateRef ,EventEmitter, Output } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { BackUpComponent } from 'src/app/shared/backUp/backUp.component';
import { PathLocatorComponent } from 'src/app/shared/pathLocator/pathLocator.component';
import { Database } from 'src/app/_models/database';
import { DBBackUpProcessInfo } from 'src/app/_models/dbBackUpProcessInfo';
import { Server } from 'src/app/_models/server';
import { NotificationService } from 'src/app/_services/notification.service';

@Component({
  selector: 'app-database-card',
  templateUrl: './database-card.component.html',
  styleUrls: ['./database-card.component.css']
})
export class DatabaseCardComponent implements OnInit {
  @Input() database: Database = <Database>{};
  @Input() server :Server = <Server>{};
  @Output() emitDBIncludeInBackUpEvent : EventEmitter<any> = new EventEmitter();
  dbBackProcessInfo : DBBackUpProcessInfo = { serverName: '', DBName:'' , DBPath:''};
  DBInfomodalRef: BsModalRef =<BsModalRef>{};
  checked = false;

  
  path :string = this.dbBackProcessInfo.DBPath;
  DBPath_modalRef: BsModalRef =<BsModalRef>{};

  constructor( private notify: NotificationService,private http: HttpClient ,private modalService: BsModalService) { }

  ngOnInit() {
    this.http.get<string>('http://localhost:5051/api/Backup/DefaultDBPath').subscribe( Response =>
    this.dbBackProcessInfo.DBPath = Response,
    )
  }

  openDBInfoModal(template: TemplateRef<any>) {
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
