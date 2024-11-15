using System.Text.Json.Serialization;
using System.Text.Json;

namespace BinaryCupcake.Client.Services
{
    public static class JsonContentService
    {
        public static string SerializeObj(object modelObject) => JsonSerializer.Serialize(modelObject, JsonOptions());
        public static T DeserializeJsonString<T>(string jsonString) => JsonSerializer.Deserialize<T>(jsonString, JsonOptions())!;
        public static StringContent GenerateStringContent(string serializeObj) => new(serializeObj, System.Text.Encoding.UTF8, "application/json");
        public static IList<T> DeserializeJsonStringList<T>(string jsonString) => JsonSerializer.Deserialize<IList<T>>(jsonString, JsonOptions())!;
        public static JsonSerializerOptions JsonOptions()
        {
            return new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                UnmappedMemberHandling = JsonUnmappedMemberHandling.Skip
            };
        }
    }
}
