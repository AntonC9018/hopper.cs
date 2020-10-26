using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Core.Behaviors;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Core
{
    public class BehaviorControlConverter : JsonConverter<BehaviorControl>
    {
        FieldInfo info_m_behaviors = typeof(BehaviorControl).GetField("m_behaviors", BindingFlags.NonPublic | BindingFlags.Instance);

        public class BehaviorData
        {
            public string type;
            public Behavior data;

            public BehaviorData(string type, Behavior data)
            {
                this.type = type;
                this.data = data;
            }
        }

        public override BehaviorControl ReadJson(JsonReader reader, System.Type objectType, BehaviorControl existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (!hasExistingValue)
            {
                System.Console.WriteLine("Weird");
            }
            JArray jarr = JArray.Load(reader);
            var m_behaviors = (Dictionary<System.Type, Behavior>)info_m_behaviors.GetValue(existingValue);
            foreach (var item in m_behaviors)
            {
                System.Console.WriteLine(item.Key.ToString());
            }
            foreach (var item in jarr)
            {
                var jobj = (JObject)item;
                var key = System.Type.GetType((string)jobj.GetValue("type"));
                var val = jobj.GetValue("data");
                var readerBeh = val.CreateReader();
                serializer.Populate(readerBeh, m_behaviors[key]);
            }
            return existingValue;
        }

        public override void WriteJson(JsonWriter writer, BehaviorControl value, JsonSerializer serializer)
        {
            JArray jarr = new JArray();
            var behaviors = (Dictionary<System.Type, Behavior>)info_m_behaviors.GetValue(value);
            foreach (var kvp in behaviors)
            {
                var behToken = JToken.FromObject(kvp.Value);
                
                if (behToken.Children().Count() != 0)
                {
                    var data = new BehaviorData(kvp.Key.ToString(), kvp.Value);
                    var token = JToken.FromObject(data);
                    jarr.Add(token);
                }
            }
            jarr.WriteTo(writer);
        }
    }
}