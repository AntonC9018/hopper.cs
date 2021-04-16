using Newtonsoft.Json.Linq;

namespace Hopper.Meta.Stats
{
    public struct FieldMetadata
    {
        public string type;
        public string defaultValue;


        public static FieldMetadata Parse(JToken jtok, ParsingContext ctx)
        {
            if (jtok is JValue jval)
            {
                if (jval.Type == JTokenType.Integer)
                {
                    return new FieldMetadata 
                    { 
                        type = "int",
                        defaultValue = jval.Value.ToString()
                    };
                }
            }
            else if (jtok is JObject jobj)
            {
                var result = new FieldMetadata();

                foreach (var kvp in jobj)
                {
                    ctx.Push(kvp.Key);
                    var type = StatType.ParseType(kvp.Key, out string actualName);
                    switch (type)
                    {
                        case KvpType.Metadata:
                            if (actualName == "type") { result.type = kvp.Value.ToString(); }
                            else if (actualName == "default") 
                            { 
                                result.defaultValue = kvp.Value.ToString(); 
                            }
                            else
                            {
                                ctx.Report($"Unexpected metadata name {kvp.Key}. Expected either @type or @default");
                            }
                            break;
                        default:
                            ctx.Report($"Parsed a {type} with name {kvp.Key}. Expected a Metadata (the name must start with a @).");
                            break;
                    }
                    ctx.Pop();
                }

                if (result.type == null)
                {
                    ctx.Report($"The field type must be specified explicitly. Alternatively, just specify an integer instead of an entire json object as the field value.");
                }

                if (result.defaultValue == null)
                {
                    result.defaultValue = $"{result.type}.Default";
                }

                return result;
            }
            ctx.Report($"Expected an object or an int, received {jtok.GetType()}.");
            return default;
        }
    }
}