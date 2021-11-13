import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { Database } from 'src/app/_models/database';
import { DBBackUpProcessInfo } from 'src/app/_models/dbBackUpProcessInfo';
import { NotificationService } from 'src/app/_services/notification.service';
import { environment } from 'src/environments/environment';
import { PathLocatorComponent } from '../pathLocator/pathLocator.component';

@Component({
  selector: 'app-backUp',
  templateUrl: './backUp.component.html',
  styleUrls: ['./backUp.component.css']
})
export class BackUpComponent implements OnInit {
  DBPath_modalRef: BsModalRef =<BsModalRef>{};

  databases : Database[] = <Database[]>{};
  dbBackProcessInfo : DBBackUpProcessInfo[] = [];
  path :string = '';
  Model: any = {};

  
  constructor( private notify: NotificationService,private http: HttpClient ,private modalService: BsModalService, public bsModalRef: BsModalRef) { }

  ngOnInit() {
    console.log("on init BackUpComponent");
    this.http.get<string>(environment.apiUrl +'api/Backup/DefaultDBPath').subscribe( Response => 
    this.path = Response );
  }


  confirm() {
    this.openPathDialog();
   
    this.bsModalRef.hide();
  }
 
  decline() {
    this.bsModalRef.hide();
  }

  openPathDialog(): void {
   
    const initialState = {
      pathModel: this.path
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
    console.log("in backup method ");
    console.log( this.path);
    this.databases.forEach(element => {
      this.dbBackProcessInfo.push({DBName: element.database_name,serverName: element.server_name,DBPath: <string>this.path});
    });
    this.notify.OnStartedBackUp(true);
    this.notify.progress('در حال انجام پشتیبان گیری..');
    console.log( this.dbBackProcessInfo);
    this.http.post('http://localhost:5051/api/Backup/Process',this.dbBackProcessInfo).subscribe(response => {
      this.notify.OnCompletedBackUp(true);
      this.notify.success('پشتیبان گیری با موفقیت انجام شد');
    }, error => {
      this.notify.OnCompletedBackUp(true);
      this.notify.fatalerror(error);
      console.log(error);
    } );
  }


}
