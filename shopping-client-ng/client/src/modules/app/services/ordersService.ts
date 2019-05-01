import { Injectable } from "@angular/core";
import { environment } from "../../../environments/environment";
import { HttpClient } from "@angular/common/http";
import { Observable, of } from "rxjs";
import { catchError, map } from 'rxjs/operators';
import { Product } from "../models/product";
import { Order } from "../models/order";
import { PushService } from "./pushService";
import { OrderWithItems } from "../models/orderWithItems";

@Injectable()
export class OrdersService {
  constructor(private _http: HttpClient,
    private _pushService: PushService) { }

  public getOrders(): Observable<OrderWithItems[]> {
    return this.requestOrders().pipe(
      map(orders =>
        orders.map(order => {
          const items = order.items.map(({ id }) => this.requestProductSafely(id));
          const shipped$ = this._pushService.getShippingStatus(order.id);
          return { order, items, shipped$ };
        })
      )
    );
  }

  private requestOrders(): Observable<Order[]> {
    const url = `${environment.ordersApiBaseUrl}orders`;

    return this._http.get<Order[]>(url);
  }

  private requestProductSafely(id: string): Observable<Product> {
    return this.requestProduct(id)
      .pipe(catchError(err => of({ id, name: 'Ooops...' })));
  }

  private requestProduct(id: string): Observable<Product> {
    const url = `${environment.productsApiBaseUrl}products/${id}`;

    return this._http.get<Product>(url);
  }
}
