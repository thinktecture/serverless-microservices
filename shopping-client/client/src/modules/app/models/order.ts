import { Observable } from "rxjs";

export interface Order {
    id: string;
    description: string;
    items: { id: string, quantity: number }[];
    shippingCreated$: Observable<boolean>;
}
