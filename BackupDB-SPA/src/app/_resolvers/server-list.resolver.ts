import {Injectable} from '@angular/core';
import {Resolve, Router, ActivatedRouteSnapshot} from '@angular/router';
import { NotificationService } from '../_services/notification.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Server } from '../_models/server';
import { ServerService } from '../_services/server.service';

@Injectable()
export class ServerListResolver implements Resolve<Server[]> {
    constructor(private serverService: ServerService, private router: Router,
        private notify: NotificationService) {}

    resolve(route: ActivatedRouteSnapshot): Observable<Server[]> {
        return this.serverService.getServers().pipe(
            catchError(error => {
                this.notify.error('Problem retrieving data');
                this.router.navigate(['/home']);
                return of(null);
            })
        );
    }
}