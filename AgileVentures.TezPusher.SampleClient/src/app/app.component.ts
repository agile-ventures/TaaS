import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { SignalRService, Block, Transaction } from './signalr.service';
import { MatTable } from '@angular/material';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})

export class AppComponent implements OnInit, OnDestroy {

  private readonly _signalRService: SignalRService;
  blocks: Block[];
  transactions: Transaction[];
  displayedColumnsTableBlocks: string[] = ['level', 'hash', 'timestamp', 'validation_pass'];
  displayedColumnsTableTransaction: string[] = ['source', 'destination', 'amount', 'timestamp', 'level'];

  constructor(signalRService: SignalRService) {
    this._signalRService = signalRService;
  }

  @ViewChild('tableBlocks') tableBlocks: MatTable<Block>;
  @ViewChild('tableTransactions') tableTransactions: MatTable<Transaction>;

  ngOnInit() {
    this.blocks = [];
    this.transactions = [];
    this._signalRService.init();

    this._signalRService.blocks.subscribe(block => {
      this.blocks.unshift(block);
      this.tableBlocks.renderRows();
    });

    this._signalRService.transactions.subscribe(transaction => {
      this.transactions.unshift(transaction);
      this.tableTransactions.renderRows();
    });
  }

  ngOnDestroy() {
    this._signalRService.blocks.unsubscribe();
    this._signalRService.transactions.unsubscribe();
  }
}
