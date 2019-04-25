export interface Order {
    id: string;
    description: string;
    items: { id: string, quantity: number }[];
    created: any; // FIXME probably string I guess?
}
