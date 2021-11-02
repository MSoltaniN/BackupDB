import { Component, OnInit } from '@angular/core';
import { AuthService } from './_services/auth.service';
import { JwtHelperService } from '@auth0/angular-jwt';
import { NotificationService } from './_services/notification.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  startedBackUp = false;
  completedBackUp = false;
  jwtHelper = new JwtHelperService();

  constructor(private authService: AuthService , private notify: NotificationService) {}

  ngOnInit() {
    const token = localStorage.getItem('token');
    if (token) {
      this.authService.decodedToken = this.jwtHelper.decodeToken(token);
    }


    this.notify.currentBackUpStarted.subscribe(backupStarted =>{ this.startedBackUp = backupStarted, this.onStarted()  });
    this.notify.currentBackupCompleted.subscribe(backupCompleted =>{ this.completedBackUp = backupCompleted , this.onCompleted() });
  }

  onStarted() {
    this.startedBackUp = true;
    setTimeout(() => {
      this.startedBackUp = false;
    }, 800);
  }

  onCompleted() {
    this.completedBackUp = true;
    setTimeout(() => {
      this.completedBackUp = false;
    }, 800);
  }


}
