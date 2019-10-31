import { Component, AfterViewInit, OnDestroy, ViewChild, ChangeDetectorRef } from '@angular/core';
import { SignalRService, Block, Transaction } from './signalr.service';
import { MatTable, MatDialogConfig, MatDialog } from '@angular/material';
import { ServiceUrlDialogComponent } from './serviceurl-dialog/serviceurl-dialog.component';
import { detectChanges } from '@angular/core/src/render3';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})

export class AppComponent implements AfterViewInit, OnDestroy {

  blocks: Block[];
  transactions: Transaction[];
  displayedColumnsTableBlocks: string[] = ['level', 'hash', 'timestamp', 'validation_pass'];
  displayedColumnsTableTransaction: string[] = ['source', 'destination', 'amount', 'timestamp', 'level'];
  defaultEndpointUrl = 'https://taas-staging.agile-ventures.com/api/';

  constructor(private signalRService: SignalRService, private dialog: MatDialog, private cdr: ChangeDetectorRef) { }

  @ViewChild('tableBlocks') tableBlocks: MatTable<Block>;
  @ViewChild('tableTransactions') tableTransactions: MatTable<Transaction>;

  ngAfterViewInit() {
    this.blocks = [];
    this.transactions = [];

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
  }
}
