export interface Order {
    id: string;
    description: string;
    items: { id: string, quantity: number }[];
    created: string;
}
