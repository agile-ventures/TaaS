import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { SignalRService, Block } from './signalr.service';
import { MatTable } from '@angular/material';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})

export class AppComponent implements OnInit, OnDestroy {

  private readonly _signalRService: SignalRService;
  blocks: Block[];
  displayedColumns: string[] = ['level', 'hash', 'timestamp', 'validation_pass'];

  constructor(signalRService: SignalRService) {
    this._signalRService = signalRService;
  }

  @ViewChild(MatTable) table: MatTable<Block>;

  ngOnInit() {
    this.blocks = [];
    this._signalRService.init();
    this._signalRService.messages.subscribe(block => {
      this.blocks.unshift(block);
      this.table.renderRows();
    });
  }

  ngOnDestroy() {
    this._signalRService.messages.unsubscribe();
  }

  // send() {
  //   this._signalRService.send(this.message).subscribe(() => { });
  // }
}
