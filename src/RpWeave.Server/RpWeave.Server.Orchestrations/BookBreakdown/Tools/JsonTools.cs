using System.ComponentModel;

namespace RpWeave.Server.Orchestrations.BookBreakdown.Tools;

public static class JsonTools
{
    [Description("Stores the given json string as a file with the given file name. Previous content is overwritten.")]
    public static async Task WriteJsonToFileAsync(
        [Description("The run id used to collerate outputs into a single run context")] string runId,
        [Description("The file name for the json")] string fileName, 
        [Description("The json content to be stored")] string json)
    {
        var path = Path.Combine($"/home/test/{runId}", fileName);
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        
        await File.WriteAllTextAsync(path, json);
    }
    
    [Description("Reads json content from a file with a given name. Returns an empty string if file does not exist.")]
    public static async Task<string> ReadJsonFromFileAsync(
        [Description("he run id used to collerate outputs into a single run context")] string runId,
        [Description("The file name of the json")] string fileName)
    {
        var path = Path.Combine($"/home/test/{runId}", fileName);
        return await File.ReadAllTextAsync(path);
    }
    
    [Description("Lists all already created json files for a given run id. Returns a comma separated list with all file names.")]
    public static string ListAllJsonFilesAsync(
        [Description("he run id used to collerate outputs into a single run context")] string runId)
    {
        if (!Directory.Exists($"/home/test/{runId}"))
        {
            return "";
        }
        var files = Directory.GetFiles($"/home/test/{runId}");
        return string.Join(", ", files);
    }
}