import { HttpClient } from '@angular/common/http';
import { Component, Input, OnInit, TemplateRef  } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
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
  dbBackProcessInfo : DBBackUpProcessInfo = { serverName: '', DBName:''};
  result :any;
  DBInfomodalRef: BsModalRef =<BsModalRef>{};
  DBPath_Confirm_modalRef: BsModalRef =<BsModalRef>{};
  checked = false;
  constructor( private notify: NotificationService,private http: HttpClient ,private modalService: BsModalService) { }

  ngOnInit() {
  }

  backup(){
    this.dbBackProcessInfo.DBName = this.database.database_name;
    this.dbBackProcessInfo.serverName = this.server.serverName;
    this.notify.warning('backup in progress');
    this.http.post('http://localhost:5051/api/Backup/Process',this.dbBackProcessInfo).subscribe(response => {
      this.result = response;
      console.log('result of Process:'+this.result);
      this.notify.success('backup successfully');
    }, error => {
      this.notify.error(error);
      console.log(error);
    } );
  }

  openDBInfoModal(template: TemplateRef<any>) {
    this.DBInfomodalRef = this.modalService.show(template);
  }

  openDBPath_Confirm_Modal(template: TemplateRef<any>)
  {
    this.DBPath_Confirm_modalRef = this.modalService.show(template, {class: 'modal-sm'});
    
  }

  confirm() {
    this.backup();
    this.DBPath_Confirm_modalRef.hide();
  }
 
  decline() {
    this.DBPath_Confirm_modalRef.hide();
  }



}
