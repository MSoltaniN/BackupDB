import { Component, Input, OnInit } from '@angular/core';
import { Server } from 'src/app/_models/server';

@Component({
  selector: 'app-database-list',
  templateUrl: './database-list.component.html',
  styleUrls: ['./database-list.component.css']
})
export class DatabaseListComponent implements OnInit {
  @Input() server: Server;
  constructor() { }

  ngOnInit() {
  }

}
