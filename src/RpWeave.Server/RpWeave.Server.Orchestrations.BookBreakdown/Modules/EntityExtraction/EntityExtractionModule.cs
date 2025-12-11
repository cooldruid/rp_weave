using System.Text.RegularExpressions;
using RpWeave.Server.Core.Startup;
using RpWeave.Server.Integrations.Ollama.Chat;

namespace RpWeave.Server.Orchestrations.BookBreakdown.Modules.EntityExtraction;

[ScopedService]
public class EntityExtractionModule(OllamaChatClient chatClient)
{
    public async Task<EntityExtractionResponse> ProcessAsync(List<string> inputs)
    {
        // Extract
        var extractions = new List<EntityExtractionResponseElement>();
        var index = 0;
        do
        {
            var extractionInput = PrepareExtractionInput(inputs, ref index);

            var response = await SendToModelAsync(extractionInput);

            var elements = ParseResponse(response);

            extractions.AddRange(elements);

            // give recover time
            Thread.Sleep(5000);
        } while (index < inputs.Count);

        // Deduplicate
        extractions = extractions
            .GroupBy(e => (Normalize(e.Name), e.Type))
            .Select(g => g.First())
            .ToList();

        return new EntityExtractionResponse(extractions);
    }

    private static List<EntityExtractionResponseElement> ParseResponse(string response)
    {
        var stringElements = response.Split(",");
        var elements = stringElements.Select(x =>
        {
            var parts = x.Split(':');
            return parts.Length != 2 ? null : new EntityExtractionResponseElement(parts[0].Trim(), parts[1].Trim());
        }).Where(x => x != null).ToList();
        
        return elements!;
    }

    private async Task<string> SendToModelAsync(string extractionInput)
    {
        var chatRequest =
            new OllamaChatRequest(EntityExtractionPrompts.SystemPrompt,
                EntityExtractionPrompts.UserPrompt(extractionInput), []);

        var response = await chatClient.SendAsync(chatRequest);
        return response;
    }

    private static string PrepareExtractionInput(List<string> inputs, ref int index)
    {
        var extractionInput = "";

        for (var i = 0; i < 5; i++)
        {
            if (index < inputs.Count &&
                extractionInput.Length < 15000 &&
                inputs[index].Count(x => x == '>') < 10)
            {
                extractionInput += inputs[index] + "\n\n";
            }
                
            index++;
        }

        return extractionInput;
    }

    private static string Normalize(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return string.Empty;

        var normalized = name.ToLowerInvariant();
        normalized = Regex.Replace(normalized, @"[^\w\s]", "");
        normalized = Regex.Replace(normalized, @"\s+", " ").Trim();

        return normalized;
    }
}