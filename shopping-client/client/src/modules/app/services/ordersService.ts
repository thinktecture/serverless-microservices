import { Injectable } from "@angular/core";
import { environment } from "../../../environments/environment";
import { HttpClient } from "@angular/common/http";
import { Observable, from, of } from "rxjs";
import { switchMap, toArray, catchError, concatMap } from 'rxjs/operators';

@Injectable()
export class OrdersService {
  constructor(private _http: HttpClient) {}

  public getOrders(): Observable<any[]> {

    const ordersList = this._http.get<any>(environment.ordersApiBaseUrl + "orders")
      .pipe(
        switchMap(orders => from(orders)),
        // TODO: loop through all items
        concatMap((order: any) => this._http.get<any>(environment.productsApiBaseUrl + "products/" + order.items[0].id),
          (order, items) => Object.assign(order, {items: [items]})
        ),
        toArray(),
        catchError((err) => {
          console.log(`Inner error: ${err}`);
          // TODO: what should we do in case of errors?
          // Not being able to get the products info should not break the client!
          return of([]);
      }));

      return ordersList;
  }
}
