export type PostChatMessageModel = {
    campaignId: string;
    collectionName: string;
    prompt: string;
    chatHistory: ChatHistoryLine[];
}

export type ChatHistoryLine = {
    type: string;
    message: string;
    order: number;
}