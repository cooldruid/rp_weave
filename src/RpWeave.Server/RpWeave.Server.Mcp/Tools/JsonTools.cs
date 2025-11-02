using System.ComponentModel;
using ModelContextProtocol.Server;

namespace RpWeave.Server.Mcp.Tools;

[McpServerToolType]
public static class JsonTools
{
    [McpServerTool, Description("Stores the given json string as a file with the given file name.")]
    public static async Task WriteJsonToFileAsync(
        [Description("The file name for the json")] string fileName, 
        [Description("The json content to be stored")] string json)
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), fileName);
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        
        await using var streamWriter = new StreamWriter(path);
        
        await streamWriter.WriteLineAsync(json);
    }
}