using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Hopper.Core
{
    public class IHaveIdConverter<T> : JsonConverter<T> where T : IHaveId
    {
        public override T ReadJson(JsonReader reader, System.Type objectType, T existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var id = reader.ReadAsInt32() ?? 0;
            return Registry.Default.GetKind<T>(id);
        }

        public override void WriteJson(JsonWriter writer, T value, JsonSerializer serializer)
        {
            JToken t = JToken.FromObject(value.Id);
            t.WriteTo(writer);
        }
    }
}