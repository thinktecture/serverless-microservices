import { Component, OnInit } from "@angular/core";
import { OrdersService } from "../../services/ordersService";
import { PushService } from "../../services/pushService";
import { Observable } from "rxjs";
import { Order } from "../../models/order";
import { Product } from "../../models/product";

@Component({
  selector: "app-order-list",
  templateUrl: "orderList.html"
})
export class OrderListComponent implements OnInit {
  public ordersWithItems$: Observable<{ order: Order; items: Observable<Product>[] }[]>;

  constructor(
    private _orderService: OrdersService,
    private _pushService: PushService
  ) {
    this._pushService.orderCreated.subscribe(_ => {
      this.loadOrders();
    });
  }

  private loadOrders() {
    this.ordersWithItems$ = this._orderService.getOrders();
  }

  public ngOnInit(): void {
    this.loadOrders();
  }
}
