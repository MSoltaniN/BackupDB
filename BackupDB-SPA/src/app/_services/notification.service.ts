import { Injectable } from '@angular/core';
//declare let alertify: any;
import { ToastrService } from 'ngx-toastr';
import {BehaviorSubject} from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {

  backupStarted = new BehaviorSubject <boolean> (false);
  currentBackUpStarted = this.backupStarted.asObservable();
  backupCompleted = new BehaviorSubject <boolean> (false);
  currentBackupCompleted = this.backupCompleted.asObservable();

  constructor(private toastr: ToastrService) {}

  OnStartedBackUp( started :boolean )
  {
    this.backupStarted.next(started);
  }
  
  OnCompletedBackUp( completed :boolean )
  {
    this.backupCompleted.next(completed);
  }
  
  confirm(message: string, okCallback: () => any) {
//
  }

  success(message: string) {
   // alertify.success(message);
   this.toastr.success(message)
  }

  error(message: string) {
   // alertify.error(message);
   this.toastr.error(message)
  }

  warning(message: string) {
   // alertify.warning(message);
   this.toastr.warning(message)
  }

  message(message: string) {
   // alertify.message(message);
   this.toastr.info(message)
  }

  progress(message: string)
  {
    return this.toastr.warning(message,"",{  progressBar:true , closeButton:true})
  }

  fatalerror(message: string)
  {
    return this.toastr.error(message,"",{ disableTimeOut:true , closeButton:true})
  }
}
