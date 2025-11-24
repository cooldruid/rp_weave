using FluentValidation;

namespace RpWeave.Server.Api.Features.Campaign.Create;

public class CampaignCreateValidator : AbstractValidator<CampaignCreateRequest>
{
    public CampaignCreateValidator(CampaignCreateData? campaignCreateData)
    {
        RuleFor(_ => campaignCreateData)
            .NotNull()
            .ChildRules(data =>
            {
                data.RuleFor(d => d.Name).NotEmpty().WithMessage("Campaign name is required");
                data.RuleFor(d => d.Description).NotEmpty().WithMessage("Campaign description is required");
                data.RuleFor(d => d.ChapterFontSize).NotNull().Unless(d => !d.CreateEmbeddings)
                    .WithMessage("Chapter font size is required if embeddings should be created");
                data.RuleFor(d => d.SubChapterFontSize).NotNull().Unless(d => !d.CreateEmbeddings)
                    .WithMessage("Sub-chapter font size is required if embeddings should be created");
                data.RuleFor(d => d.HeaderFontSize).NotNull().Unless(d => !d.CreateEmbeddings)
                    .WithMessage("Header font size is required if embeddings should be created");
                data.RuleFor(d => d.IgnoreFooter).NotNull().Unless(d => !d.CreateEmbeddings)
                    .WithMessage("Ignore footer is required if embeddings should be created");
            });

        RuleFor(req => req.Pdf)
            .Must(pdf => pdf == null || Path.GetExtension(pdf.FileName) == ".pdf")
            .WithMessage("Campaign file must be a PDF. Alternatively, do not provide a file if you do not need to create embeddings");
        
        RuleFor(req => req.Pdf)
            .NotNull()
            .Unless(_ => !campaignCreateData?.CreateEmbeddings ?? false)
            .WithMessage("Campaign PDF file is required if embeddings should be created");
    }
}