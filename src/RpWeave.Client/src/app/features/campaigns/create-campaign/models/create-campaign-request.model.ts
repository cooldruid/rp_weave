export type CreateCampaignRequest = {
    name: string;
    description: string;
    createEmbeddings: boolean;
    chapterFontSize: number | undefined;
    subChapterFontSize: number | undefined;
    headerFontSize: number | undefined;
    ignoreFooter: boolean | undefined;
}