import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { HubConnection } from '@aspnet/signalr';
import * as signalR from '@aspnet/signalr';
import { Observable, from } from 'rxjs';
import * as uuidv4 from 'uuid/v4';
import { Subject } from 'rxjs';

export interface PushMessage {
    block_header: Block;
    transaction: Transaction;
}

export interface Block {
    level: number;
    hash: string;
    timestamp: Date;
    validation_pass: number;
    proto: number;
    predecessor: string;
    operations_hash: string;
    fitness: string[];
    context: string;
    protocol_data: string;
}

export interface Transaction {
    operation_hash: string;
    block_hash: string;
    block_level: number;
    timestamp: Date;
    transaction_content: TransactionContent;
}

export interface TransactionContent {
    source: string;
    destination: string;
    amount: string;
    fee: string;
    counter: string;
    gas_limit: string;
    storage_limit: string;
}

export interface Subscription {
    userId?: string;
    transactionAddresses: string[];
}

@Injectable()
export class SignalRService {

    private readonly _http: HttpClient;
    private readonly _baseUrl: string = '';
    private hubConnection: HubConnection;
    blocks: Subject<Block> = new Subject();
    transactions: Subject<any> = new Subject();
    userId: string;

    constructor(http: HttpClient) {
        this._http = http;
        this.userId = uuidv4();
    }

    private subscribeToTransactions(model: Subscription): Observable<any> {
        return from(this.hubConnection.send("subscribe", model));
    }

    private connect(): Observable<any> {
        this.hubConnection = new signalR.HubConnectionBuilder()
        .withUrl(`${this._baseUrl}/tezosHub`)
        .configureLogging(signalR.LogLevel.Information)
        .build();

    return from(this.hubConnection.start());
    }

    init() {
        console.log(`initializing SignalRService...`);
        this.connect().subscribe(() => {
            console.log(`subscribing to all transactions`);
            this.subscribeToTransactions({ transactionAddresses: ['all'] })
                .subscribe(() => {
                    console.log(`subscribed to all transactions`);
                });
    
            this.hubConnection.on('block_headers', (data: PushMessage) => {
                this.blocks.next(data.block_header);
            });
    
            this.hubConnection.on('transactions', (data: PushMessage) => {
                this.transactions.next(data.transaction);
            });
        })
    }
}
