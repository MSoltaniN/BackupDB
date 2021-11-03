import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BsDropdownModule  } from 'ngx-bootstrap/dropdown';
import {AccordionModule   } from 'ngx-bootstrap/accordion';
import { TooltipModule  } from 'ngx-bootstrap/tooltip';
import {  ModalModule  } from 'ngx-bootstrap/modal';
import { RouterModule } from '@angular/router';
import { ToastrModule } from 'ngx-toastr';
import { MatDialogModule } from '@angular/material/dialog';
import {MatSelectModule} from '@angular/material/select';
import {MatCheckboxModule} from '@angular/material/checkbox';
import { MatInputModule } from '@angular/material/input';
import { NgProgressModule  } from 'ngx-progressbar';
import { NgProgressHttpModule } from 'ngx-progressbar/http';


import { AppComponent } from './app.component';
import { NavComponent } from './nav/nav.component';
import { AuthService } from './_services/auth.service';
import { HomeComponent } from './home/home.component';
import { RegisterComponent } from './register/register.component';
import { ErrorInterceptorProvider } from './_services/error.interceptor';
import { NotificationService } from './_services/notification.service';
import { ServerListComponent } from './server-list/server-list.component';
import { appRoutes } from './routes';
import { AuthGuard } from './_guards/auth.guard';
import { DatabaseCardComponent } from './server-list/database-list/database-card/database-card.component';
import { ServerService } from './_services/server.service';
import { ServerListResolver } from './_resolvers/server-list.resolver';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { toArrayPipe } from './_pipes/toArray.pipe';
import { PathLocatorComponent } from './shared/pathLocator/pathLocator.component';
import { BackUpComponent } from './shared/backUp/backUp.component';
import { DatabaseListComponent } from './server-list/database-list/database-list.component';
import { FileSizePipe } from './_pipes/file-size.pipe';
import { JaliliPipe } from './_pipes/jalili.pipe';
//import { DatabaseCardResolver } from './_resolvers/database-card.resolver';

@NgModule({
  declarations: [
    AppComponent,
    NavComponent,
    HomeComponent,
    RegisterComponent,
    ServerListComponent,
    DatabaseCardComponent,
    PathLocatorComponent,
    toArrayPipe,
    BackUpComponent,
    DatabaseListComponent,
    FileSizePipe,
    JaliliPipe
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    FormsModule,
    BsDropdownModule.forRoot(),
    RouterModule.forRoot(appRoutes),
    AccordionModule.forRoot(),
    TooltipModule.forRoot(),
    ModalModule.forRoot(),
    BrowserAnimationsModule,
    ReactiveFormsModule,
    ToastrModule.forRoot(),
    MatDialogModule,
    MatSelectModule ,
    MatCheckboxModule,
    MatInputModule    ,
     NgProgressModule.withConfig({
      spinnerPosition: "left",
      color: "#f71cff"
    }),
    NgProgressHttpModule
  ],
  providers: [
      AuthService,
      ErrorInterceptorProvider,
      NotificationService,
      AuthGuard,
      ServerService,
      ServerListResolver,
     // DatabaseListResolver,
    ],
  bootstrap: [AppComponent]
  ,
  entryComponents: [ PathLocatorComponent ]
})
export class AppModule {}
