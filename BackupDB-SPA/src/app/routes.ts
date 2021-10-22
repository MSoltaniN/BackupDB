import {Routes} from '@angular/router';
import { HomeComponent } from './home/home.component';
import { ServerListComponent } from './server-list/server-list.component';
import { AuthGuard } from './_guards/auth.guard';

export const appRoutes: Routes = [
    {path: 'home', component: HomeComponent},
    {
        path: '',
        runGuardsAndResolvers: 'always',
        canActivate: [AuthGuard],
        children: [
            {path: 'Servers', component: ServerListComponent},
        ]
    },
    {path: '**', redirectTo: 'home', pathMatch: 'full'},
];
