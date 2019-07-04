import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { HubConnection } from '@aspnet/signalr';
import * as signalR from '@aspnet/signalr';
import { Observable } from 'rxjs';
import { SignalRConnectionInfo } from './signalr-connection-info.model';
import { map } from 'rxjs/operators';
import { Subject } from 'rxjs';

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

@Injectable()
export class SignalRService {

    private readonly _http: HttpClient;
    private readonly _baseUrl: string = 'TODO';
    private hubConnection: HubConnection;
    messages: Subject<Block> = new Subject();

    constructor(http: HttpClient) {
        this._http = http;
    }

    private getConnectionInfo(): Observable<SignalRConnectionInfo> {
        const requestUrl = `${this._baseUrl}negotiate`;
        return this._http.get<SignalRConnectionInfo>(requestUrl);
    }

    init() {
        console.log(`initializing SignalRService...`);
        this.getConnectionInfo().subscribe(info => {
            console.log(`received info for endpoint ${info.url}`);
            const options = {
                accessTokenFactory: () => info.accessToken
            };

            this.hubConnection = new signalR.HubConnectionBuilder()
                .withUrl(info.url, options)
                .configureLogging(signalR.LogLevel.Information)
                .build();

            this.hubConnection.start().catch(err => console.error(err.toString()));

            this.hubConnection.on('blocks', (data: any) => {
                console.log(data);
                this.messages.next(data);
            });
        });
    }
}
