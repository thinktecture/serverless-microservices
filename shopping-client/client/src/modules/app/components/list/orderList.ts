import { Component, OnInit } from "@angular/core";
import { OrdersService } from "../../services/ordersService";
import { PushService } from "../../services/pushService";

@Component({
  selector: "app-order-list",
  templateUrl: "orderList.html"
})
export class OrderListComponent implements OnInit {
  public orders = [];

  constructor(
    private _orderService: OrdersService,
    private _pushService: PushService
  ) {
    this._pushService.orderCreated.subscribe(_ => {
      this.loadOrders();
    });

    this._pushService.orderShipping.subscribe(orderId => {
      const index = this.orders.findIndex(order => order.id === orderId);

      if (index !== -1) {
        const updatedOrder = this.orders[index];
        updatedOrder.shippingCreated = true;
        this.orders[index] = updatedOrder;
      }
    });
  }

  private loadOrders() {
    this._orderService.getOrders().subscribe(data => {
      this.orders = data;
    });
  }

  public ngOnInit(): void {
    this.loadOrders();
  }
}
