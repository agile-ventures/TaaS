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
    origination: Origination;
    delegation: Delegation;
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

export interface Operation {
    operation_hash: string;
    block_hash: string;
    block_level: number;
    timestamp: Date;
}

export interface OperationContent {
    source: string;
    fee: string;
    counter: string;
    gas_limit: string;
    storage_limit: string;
}

export interface Metadata {
    operation_result: OperationResults;
}

export interface OperationResults {
    originated_contracts: string[];
}


export interface Transaction extends Operation {
    transaction_content: TransactionContent;
}

export interface Origination extends Operation {
    origination_content: OriginationContent;
}

export interface Delegation extends Operation {
    delegation_content: DelegationContent;
}

export interface TransactionContent extends OperationContent {
    destination: string;
    amount: string;
}

export interface OriginationContent extends OperationContent {
    balance: string;
    amount: string;
    metadata: Metadata;
}

export interface DelegationContent extends OperationContent {
    delegate: string;
}

export interface Subscription {
    userId: string;
    transactionAddresses: string[];
    delegationAddresses: string[];
    originationAddresses: string[];
}

@Injectable()
export class SignalRService {

    private readonly _http: HttpClient;
    private baseUrl: string;
    private hubConnection: HubConnection;
    blocks: Subject<Block> = new Subject();
    transactions: Subject<any> = new Subject();
    originations: Subject<any> = new Subject();
    delegations: Subject<any> = new Subject();
    userId: string;

    constructor(http: HttpClient) {
        this._http = http;
        this.userId = uuidv4();
    }

    private getConnectionInfo(): Observable<SignalRConnectionInfo> {
        const requestUrl = `${this.baseUrl}negotiate`;
        return this._http.get<SignalRConnectionInfo>(requestUrl,
            {
                headers: {
                    'x-tezos-live-userid': this.userId
                }
            });
    }

    private subscribeToAll(model: Subscription): Observable<any> {
        const requestUrl = `${this.baseUrl}subscribe`;
        const httpOptions = {
            headers: new HttpHeaders({
                'Content-Type': 'application/json',
            })
        };
        return this._http.post(requestUrl, model, httpOptions);
    }

    init(endpointUrl: string) {
        console.log(`initializing SignalRService...`);
        this.baseUrl = endpointUrl;
        this.getConnectionInfo().subscribe(info => {
            console.log(`received info for endpoint ${info.url}`);
            const options = {
                accessTokenFactory: () => info.accessToken
            };

            console.log(`subscribing to all transactions, originations and delegations`);
            const model = <Subscription>{
                userId: this.userId,
                transactionAddresses: ['all'],
                delegationAddresses: ['all'],
                originationAddresses: ['all']
            };
            this.subscribeToAll(model)
                .subscribe(() => {
                    console.log(`subscribed to all transactions, originations and delegations`);
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

            this.hubConnection.on('originations', (data: PushMessage) => {
                this.originations.next(data.origination);
            });

            this.hubConnection.on('delegations', (data: PushMessage) => {
                this.delegations.next(data.delegation);
            });
        });
    }
}
