import {Routes} from '@angular/router';
import { HomeComponent } from './home/home.component';
//import { DatabaseListComponent } from './server-list//database-list/database-card/database-card.component';
import { ServerListComponent } from './server-list/server-list.component';
import { AuthGuard } from './_guards/auth.guard';
//import { DatabaseListResolver } from './_resolvers/database-card.resolver';
import { ServerListResolver } from './_resolvers/server-list.resolver';

export const appRoutes: Routes = [
    {path: 'home', component: HomeComponent},
    {
        path: '',
        runGuardsAndResolvers: 'always',
        canActivate: [AuthGuard],
        children: [
            {path: 'api/BackUp/servers', component: ServerListComponent/*  , resolve: { servers: ServerListResolver} */ },
           // {path: 'databases', component: DatabaseListComponent , resolve: { Databases: DatabaseListResolver} },
        ]
    },
    {path: '**', redirectTo: 'home', pathMatch: 'full'},
];
