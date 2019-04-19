import { Injectable } from "@angular/core";
import { HubConnection, IHttpConnectionOptions } from "@aspnet/signalr";
import { environment } from "../../../environments/environment";
import { SignalRConnectionInformation } from "../models/signalRConnectionInformation";
import { HttpClient } from "@angular/common/http";
import { Observable, Subject } from "rxjs";
import * as signalR from "@aspnet/signalr";

@Injectable()
export class PushService {
  private _ordersHubConnection: HubConnection;
  private _shippingsHubConnection: HubConnection;

  public orderShipping: Subject<string> = new Subject();
  public orderCreated: Subject<string> = new Subject();

  constructor(private _http: HttpClient) { }

  private async restartConnection(connection: HubConnection) {
    try {
      await connection.start();
    } catch (err) {
      console.log(err);
      setTimeout(() => this.restartConnection(connection), 2000);
    }
  };

  public start(): void {
    this.getConnectionInfo("ordersHub").subscribe(config => {
      console.log(`Received info for Orders endpoint ${config.url}`);

      const options: IHttpConnectionOptions = {
        accessTokenFactory: () => config.accessToken
      };

      this._ordersHubConnection = new signalR.HubConnectionBuilder()
        .withUrl(config.url, options)
        .configureLogging(signalR.LogLevel.Information)
        .build();

      this._ordersHubConnection
        .start()
        .then(() => {
          console.log("Orders SignalR connection established.");

          this._ordersHubConnection.on("orderCreated", message => {
            this.orderCreated.next(message.orderId);
          });
        })
        .catch(err =>
          console.error("Orders SignalR connection not established. " + err)
        );

      this._ordersHubConnection.onclose(async () => {
        await this.restartConnection(this._ordersHubConnection);
      });
    });

    this.getConnectionInfo("shippingsHub").subscribe(config => {
      console.log(`Received info for Shippings endpoint ${config.url}`);

      const options: IHttpConnectionOptions = {
        accessTokenFactory: () => config.accessToken
      };

      this._shippingsHubConnection = new signalR.HubConnectionBuilder()
        .withUrl(config.url, options)
        .configureLogging(signalR.LogLevel.Information)
        .build();

      this._shippingsHubConnection
        .start()
        .then(() => {
          console.log("Shippings SignalR connection established.");

          this._shippingsHubConnection.on("shippingInitiated", message => {
            this.orderShipping.next(message.orderId);
          });
        })
        .catch(err =>
          console.error("Shippings SignalR connection not established. " + err)
        );

        this._shippingsHubConnection.onclose(async () => {
          await this.restartConnection(this._shippingsHubConnection);
        });
    });
  }

  public stop(): void {
    if (this._ordersHubConnection) {
      this._ordersHubConnection.stop();
    }

    this._ordersHubConnection = undefined;

    if (this._shippingsHubConnection) {
      this._shippingsHubConnection.stop();
    }

    this._shippingsHubConnection = undefined;
  }

  private getConnectionInfo(
    hubName: string): Observable<SignalRConnectionInformation> {
    const requestUrl = `${environment.notificationsServiceBaseUrl}config/${hubName}`;

    return this._http.get<SignalRConnectionInformation>(requestUrl);
  }
}
