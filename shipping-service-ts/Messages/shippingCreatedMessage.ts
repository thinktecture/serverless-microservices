import { Guid } from "guid-typescript";

export class ShippingCreatedMessage {
    Id: Guid;
    Created: string;
    OrderId: string;
    UserId: string;
}