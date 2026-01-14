using System.Text.Json;

namespace RpWeave.Server.Core.Extensions;

public static class StringExtensions
{
    public static T? JsonDeserializeSafe<T>(this string json)
    {
        try
        {
            var obj = JsonSerializer.Deserialize<T>(
                json,
                new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                });

            return obj;
        }
        catch (Exception ex)
        {
            return default;
        }
    }
}