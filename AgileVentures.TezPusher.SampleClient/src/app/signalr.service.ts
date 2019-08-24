import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { HubConnection } from '@aspnet/signalr';
import * as signalR from '@aspnet/signalr';
import { Observable } from 'rxjs';
import { SignalRConnectionInfo } from './signalr-connection-info.model';
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
    userId: string;
    transactionAddresses: string[];
}

@Injectable()
export class SignalRService {

    private readonly _http: HttpClient;
    private readonly _baseUrl: string = 'https://taas-staging.agile-ventures.com/api/';
    // private readonly _baseUrl: string = 'https://taas.agile-ventures.com/api/';
    // private readonly _baseUrl: string = 'http://localhost:7071/api/';
    private hubConnection: HubConnection;
    blocks: Subject<Block> = new Subject();
    transactions: Subject<any> = new Subject();
    userId: string;

    constructor(http: HttpClient) {
        this._http = http;
        this.userId = uuidv4();
    }

    private getConnectionInfo(): Observable<SignalRConnectionInfo> {
        const requestUrl = `${this._baseUrl}negotiate`;
        return this._http.get<SignalRConnectionInfo>(requestUrl,
            {
                headers: {
                    'x-tezos-live-userid': this.userId
                }
            });
    }

    private subscribeToTransactions(model: Subscription): Observable<any> {
        const requestUrl = `${this._baseUrl}subscribe`;
        const httpOptions = {
            headers: new HttpHeaders({
                'Content-Type': 'application/json',
            })
        };
        return this._http.post(requestUrl, model, httpOptions);
    }

    init() {
        console.log(`initializing SignalRService...`);
        this.getConnectionInfo().subscribe(info => {
            console.log(`received info for endpoint ${info.url}`);
            const options = {
                accessTokenFactory: () => info.accessToken
            };

            console.log(`subscribing to all transactions`);
            this.subscribeToTransactions({ userId: this.userId, transactionAddresses: ['all'] })
                .subscribe(() => {
                    console.log(`subscribed to all transactions`);
                });

            this.hubConnection = new signalR.HubConnectionBuilder()
                .withUrl(info.url, options)
                .configureLogging(signalR.LogLevel.Information)
                .build();

            this.hubConnection.start().catch(err => console.error(err.toString()));

            this.hubConnection.on('block_headers', (data: PushMessage) => {
                this.blocks.next(data.block_header);
            });

            this.hubConnection.on('transactions', (data: PushMessage) => {
                this.transactions.next(data.transaction);
            });
        });
    }
}
