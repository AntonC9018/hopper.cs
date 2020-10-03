using Core.Items;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Core
{
    public class InventoryConverter : JsonConverter<IInventory>
    {
        public override IInventory ReadJson(JsonReader reader, System.Type objectType, IInventory existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JArray jarr = JArray.Load(reader);
            foreach (var item in jarr)
            {
                existingValue.Equip(serializer.Deserialize<IItem>(item.CreateReader()));
            }
            return existingValue;
        }

        public override void WriteJson(JsonWriter writer, IInventory value, JsonSerializer serializer)
        {
            JArray jarr = new JArray();
            foreach (IItem item in value.AllItems)
            {
                System.Console.WriteLine("next Item");
                jarr.Add(JToken.FromObject(item, serializer));
            }
            jarr.WriteTo(writer);
        }
    }
}