import { Component, Inject, OnInit, Output ,EventEmitter} from '@angular/core';
import {MatDialog, MatDialogRef, MAT_DIALOG_DATA} from '@angular/material/dialog';
import { BsModalRef } from 'ngx-bootstrap/modal';


@Component({
  selector: 'app-pathLocator',
  templateUrl: './pathLocator.component.html',
  styleUrls: ['./pathLocator.component.css']
})
export class PathLocatorComponent implements OnInit {
  oldPath: string = '';
  pathModel: string = '';
  public passEntryEvent: EventEmitter<any> = new EventEmitter();

  constructor( public bsModalRef: BsModalRef) { }

  ngOnInit() {
    this.oldPath = this.pathModel;
  }

  onNoClick(): void {
    this.bsModalRef.hide();
  }

  ConfirmPath(){
    this.passEntryEvent.emit( { data: this.pathModel, res:200} );
    this.bsModalRef.hide();
  }
}
