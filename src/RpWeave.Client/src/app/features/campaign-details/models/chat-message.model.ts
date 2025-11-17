export type ChatMessageModel = {
    order: number;
    content: string;
    type: 'user' | 'model';
}