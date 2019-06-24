import { Component, OnInit } from "@angular/core";
import { OrdersService } from "../../services/ordersService";
import { PushService } from "../../services/pushService";
import { Observable } from "rxjs";
import { OrderWithItems } from "../../models/orderWithItems";

@Component({
  selector: "app-order-list",
  templateUrl: "orderList.html",
  styleUrls: ["orderList.scss"]
})
export class OrderListComponent implements OnInit {
  public ordersWithItems$: Observable<OrderWithItems[]>;

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
