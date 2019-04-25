import { Order } from "./order";
import { Product } from "./product";
import { Observable } from "rxjs";

export interface OrderWithItems {
    order: Order;
    items: Observable<Product>[];
    shipped$: Observable<boolean>;
}
