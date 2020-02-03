import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { SignalRService, Block, Transaction, Origination, Delegation } from './signalr.service';
import { MatTable } from '@angular/material';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})

export class AppComponent implements OnInit, OnDestroy {

  private readonly _signalRService: SignalRService;
  blocks: Block[] = [];
  transactions: Transaction[] = [];
  originations: Origination[] = [];
  delegations: Delegation[] = [];
  displayedColumnsTableBlocks: string[] = ['level', 'hash', 'timestamp', 'validation_pass'];
  displayedColumnsTableTransaction: string[] = ['source', 'destination', 'amount', 'timestamp', 'level'];
  displayedColumnsTableOrigination: string[] = ['source', 'originated', 'fee', 'timestamp', 'level'];
  displayedColumnsTableDelegation: string[] = ['source', 'delegate', 'fee', 'timestamp', 'level'];


  constructor(signalRService: SignalRService) {
    this._signalRService = signalRService;
  }

  @ViewChild('tableBlocks', {static: false}) tableBlocks: MatTable<Block>;
  @ViewChild('tableTransactions', {static: false}) tableTransactions: MatTable<Transaction>;
  @ViewChild('tableOriginations', {static: false}) tableOriginations: MatTable<Origination>;
  @ViewChild('tableDelegations', {static: false}) tableDelegations: MatTable<Delegation>;

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

    this._signalRService.originations.subscribe(origination => {
      this.originations.unshift(origination);
      this.tableOriginations.renderRows();
    });

    this._signalRService.delegations.subscribe(delegation => {
      this.delegations.unshift(delegation);
      this.tableDelegations.renderRows();
    });
  }

  ngOnDestroy() {
    this._signalRService.blocks.unsubscribe();
    this._signalRService.transactions.unsubscribe();
  }
}
