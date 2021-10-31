import { HttpClient } from '@angular/common/http';
import { Component, Input, OnInit, TemplateRef ,EventEmitter } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
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
  dbBackProcessInfo : DBBackUpProcessInfo = { serverName: '', DBName:'' , DBPath:''};
  result :any;
  DBInfomodalRef: BsModalRef =<BsModalRef>{};
  DBPath_Confirm_modalRef: BsModalRef =<BsModalRef>{};
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

  openDBPath_Confirm_Modal(template: TemplateRef<any>)
  {
    this.DBPath_Confirm_modalRef = this.modalService.show(template, {class: 'modal-sm'});
  }

  confirm() {
    this.openPathDialog();
   
    this.DBPath_Confirm_modalRef.hide();
  }
 
  decline() {
    this.DBPath_Confirm_modalRef.hide();
  }

  openPathDialog(): void {
    const initialState = {
      pathModel: 
        [this.dbBackProcessInfo.DBPath]
  };
    this.DBPath_modalRef = this.modalService.show(PathLocatorComponent, {initialState});
    this.DBPath_modalRef.content.passEntryEvent.subscribe((res: { data: string; }) =>
      {
        this.path = res.data
        this.backup();
      }
    )
  }

  backup(){
    this.dbBackProcessInfo.DBName = this.database.database_name;
    this.dbBackProcessInfo.serverName = this.server.serverName;
    this.dbBackProcessInfo.DBPath = this.path;
    this.notify.warning('در حال انجام پشتیبان گیری..');
    this.http.post('http://localhost:5051/api/Backup/Process',this.dbBackProcessInfo).subscribe(response => {
      this.result = response;
      this.notify.success('پشتیبان گیری با موفقیت انجام شد');
    }, error => {
      this.notify.error(error);
      console.log(error);
    } );
  }

}
