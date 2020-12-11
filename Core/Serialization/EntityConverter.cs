using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Hopper.Core
{
    // public class EntityConverter : JsonConverter<Entity>
    // {
    //     public override Entity ReadJson(
    //         JsonReader reader,
    //         System.Type objectType,
    //         Entity existingValue,
    //         bool hasExistingValue,
    //         JsonSerializer serializer)
    //     {
    //         if (hasExistingValue)
    //         {
    //             System.Console.WriteLine(existingValue);
    //             throw new JsonException();
    //         }

    //         JObject jobj = JObject.Load(reader);

    //         int id = (int)jobj.GetValue("Id");
    // int factoryId = Registry.Default.Entity.MapMetadata(id).factoryId;
    // IFactory<Entity> factory = Registry.Default.EntityFactory.Get(factoryId);
    // Entity entity = factory.ReInstantiate(null, id);
    // serializer.Populate(jobj.CreateReader(), entity);

    // return (Entity)entity;
    //     }

    //     public override void WriteJson(JsonWriter writer, Entity value, JsonSerializer serializer)
    //     {
    //         throw new System.NotImplementedException();
    //     }

    //     public override bool CanWrite => false;
    // }
}