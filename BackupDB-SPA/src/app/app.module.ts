import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { BsDropdownModule } from 'ngx-bootstrap';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavComponent } from './nav/nav.component';
import { AuthService } from './_services/auth.service';
import { HomeComponent } from './home/home.component';
import { RegisterComponent } from './register/register.component';
import { ErrorInterceptorProvider } from './_services/error.interceptor';
import { AlertifyService } from './_services/alertify.service';
import { ServerListComponent } from './server-list/server-list.component';
import { appRoutes } from './routes';
import { AuthGuard } from './_guards/auth.guard';
import { DatabaseListComponent } from './server-list/database-list/database-list.component';
//import { DatabaseCardComponent } from './server-list/database-list/database-card/database-card.component';
import { ServerService } from './_services/server.service';
import { ServerListResolver } from './_resolvers/server-list.resolver';
//import { DatabaseListResolver } from './_resolvers/database-list.resolver';

@NgModule({
  declarations: [
    AppComponent,
    NavComponent,
    HomeComponent,
    RegisterComponent,
    ServerListComponent,
    DatabaseListComponent,
   // DatabaseCardComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    FormsModule,
    BsDropdownModule.forRoot(),
    RouterModule.forRoot(appRoutes)
  ],
  providers: [
      AuthService,
      ErrorInterceptorProvider,
      AlertifyService,
      AuthGuard,
      ServerService,
      ServerListResolver,
     // DatabaseListResolver,
    ],
  bootstrap: [AppComponent]
})
export class AppModule {}
