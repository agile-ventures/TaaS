import { Component, OnDestroy, ViewChild, ChangeDetectorRef, AfterViewInit } from '@angular/core';
import { SignalRService, Block, Transaction, Origination, Delegation } from './signalr.service';
import { MatTable, MatDialogConfig, MatDialog } from '@angular/material';
import { ServiceUrlDialogComponent } from './serviceurl-dialog/serviceurl-dialog.component';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})

export class AppComponent implements AfterViewInit, OnDestroy {

  blocks: Block[] = [];
  transactions: Transaction[] = [];
  originations: Origination[] = [];
  delegations: Delegation[] = [];
  displayedColumnsTableBlocks: string[] = ['level', 'hash', 'timestamp', 'validation_pass'];
  displayedColumnsTableTransaction: string[] = ['source', 'destination', 'amount', 'timestamp', 'level'];
  displayedColumnsTableOrigination: string[] = ['source', 'originated', 'fee', 'timestamp', 'level'];
  displayedColumnsTableDelegation: string[] = ['source', 'delegate', 'fee', 'timestamp', 'level'];

  defaultEndpointUrl = 'https://taas-staging.tezoslive.io/api/';

  constructor(private signalRService: SignalRService, private dialog: MatDialog, private cdr: ChangeDetectorRef) { }

  @ViewChild('tableBlocks', {static: false}) tableBlocks: MatTable<Block>;
  @ViewChild('tableTransactions', {static: false}) tableTransactions: MatTable<Transaction>;
  @ViewChild('tableOriginations', {static: false}) tableOriginations: MatTable<Origination>;
  @ViewChild('tableDelegations', {static: false}) tableDelegations: MatTable<Delegation>;

  ngAfterViewInit() {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.data = { endpoint: this.defaultEndpointUrl };

    setTimeout(() => {
      const dialogRef = this.dialog.open(ServiceUrlDialogComponent, dialogConfig);
      dialogRef.afterClosed().subscribe(this.onDialogClose);
    }, 0);
    this.cdr.detectChanges();
  }

  ngOnDestroy() {
    this.signalRService.blocks.unsubscribe();
    this.signalRService.transactions.unsubscribe();
    this.signalRService.originations.unsubscribe();
    this.signalRService.delegations.unsubscribe();
  }

  private onDialogClose = (dialogEndpointUrl: string) => {

    if (dialogEndpointUrl && !dialogEndpointUrl.endsWith('/')) {
      dialogEndpointUrl += '/';
    }
    this.signalRService.init(dialogEndpointUrl ? dialogEndpointUrl : this.defaultEndpointUrl);

    this.signalRService.blocks.subscribe(block => {
      this.blocks.unshift(block);
      this.tableBlocks.renderRows();
    });

    this.signalRService.transactions.subscribe(transaction => {
      this.transactions.unshift(transaction);
      this.tableTransactions.renderRows();
    });

    this.signalRService.originations.subscribe(origination => {
      this.originations.unshift(origination);
      this.tableOriginations.renderRows();
    });

    this.signalRService.delegations.subscribe(delegation => {
      this.delegations.unshift(delegation);
      this.tableDelegations.renderRows();
    });
  }
}
