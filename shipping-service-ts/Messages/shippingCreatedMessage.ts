import { Guid } from "guid-typescript";

export class ShippingCreatedMessage {
    id: Guid;
    created: string;
    orderId: string;
    userId: string;
}