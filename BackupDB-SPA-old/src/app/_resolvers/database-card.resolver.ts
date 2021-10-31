// import {Injectable} from '@angular/core';
// import {Resolve, Router, ActivatedRouteSnapshot} from '@angular/router';
// import { AlertifyService } from '../_services/alertify.service';
// import { Observable, of } from 'rxjs';
// import { catchError } from 'rxjs/operators';
// import { ServerService } from '../_services/server.service';
// import { Database } from '../_models/database';

// @Injectable()
// export class DatabaseCardResolver implements Resolve<Database> {
//     constructor(private serverService: ServerService, private router: Router,
//         private alertify: AlertifyService) {}

//     resolve(route: ActivatedRouteSnapshot): Observable<Database> {
//         return this.serverService.getDatabases(route.params['name']).pipe(
//             catchError(error => {
//                 this.alertify.error('Problem retrieving data');
//                 this.router.navigate(['/home']);
//                 return of(null);
//             })
//         );
//     }
// }