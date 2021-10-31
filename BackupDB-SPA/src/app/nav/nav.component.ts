import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { NotificationService } from '../_services/notification.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {};

  constructor(public authService: AuthService, private notify: NotificationService,
      private router: Router) { }

  ngOnInit() {
  }

  login() {
    this.authService.login(this.model).subscribe(next => {
      this.notify.success('Logged in successfully');
    }, error => {
      this.notify.error(error);
    }, () => {
      this.router.navigate(['api/BackUp/servers']);
    });
  }

  loggedIn() {
    const token = localStorage.getItem('token');
    return !!token;
  }

  logout() {
    localStorage.removeItem('token');
    this.notify.message('logged out');
    this.router.navigate(['/home']);
  }

}
