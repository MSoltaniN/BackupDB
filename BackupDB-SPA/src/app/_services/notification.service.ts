import { Injectable } from '@angular/core';
//declare let alertify: any;
import { ToastrService } from 'ngx-toastr';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  constructor(private toastr: ToastrService) {}

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
}
