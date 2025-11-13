using FluentValidation;

namespace RpWeave.Server.Api.Features.Campaign.Create;

public class CampaignCreateValidator : AbstractValidator<CampaignCreateRequest>
{
    public CampaignCreateValidator()
    {
        RuleFor(req => req.Data)
            .ChildRules(data =>
                {
                    data.RuleFor(d => d.Name).NotEmpty().WithMessage("Campaign name is required");
                    data.RuleFor(d => d.Description).NotEmpty().WithMessage("Campaign description is required");
                }
            );

        RuleFor(req => req.Pdf)
            .Must(pdf => pdf == null || Path.GetExtension(pdf.FileName) == ".pdf")
            .WithMessage("Campaign file must be a PDF. Alternatively, do not provide a file if you do not need to create embeddings");
        
        RuleFor(req => req.Pdf)
            .NotNull()
            .Unless(req => !req.Data.CreateEmbeddings)
            .WithMessage("Campaign PDF file is required if embeddings should be created");
    }
}