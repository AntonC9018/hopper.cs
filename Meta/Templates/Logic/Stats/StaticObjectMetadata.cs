using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Hopper.Meta.Stats
{
    public struct FieldAssignment
    {
        public string name;
        public string defaultValue;

        public FieldAssignment(string name, string defaultValue)
        {
            this.name = name;
            this.defaultValue = defaultValue;
        }
    }
    
    public struct StaticStatFieldMetadata
    {
        public Scope<StatType> scope;
        public List<FieldAssignment> fields;

        public string FieldCommaJoin(System.Func<FieldAssignment, string> func)
        {
            return System.String.Join(", ", fields.Select(func));
        }

        public static StaticStatFieldMetadata Parse(JToken jtok, ParsingContext ctx)
        {
            if (jtok is JObject jobj)
            {
                var result = new StaticStatFieldMetadata();
                result.fields = new List<FieldAssignment>();

                foreach (var kvp in jobj)
                {
                    ctx.Push(kvp.Key);
                    var type = StatType.ParseType(kvp.Key, out string actualName);
                    switch (type)
                    {
                        case KvpType.Metadata:
                            if (actualName == "type") 
                            { 
                                var typeName = (string)kvp.Value;
                                result.scope = ctx.scope.Lookup(typeName.Split('.'));
                            }
                            else if (actualName == "default") 
                            { 
                                result.Populate(kvp.Value as JObject, ctx); 
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

                if (result.scope == null)
                {
                    ctx.Report($"The field type must be specified explicitly. Alternatively, just specify an integer instead of an entire json object as the field value.");
                }

                return result;
            }
            ctx.Report($"Expected an object, received {jtok.GetType()}.");
            return default;
        }

        public void Populate(JObject jobj, ParsingContext ctx)
        {
            foreach (var kvp in jobj)
            {
                ctx.Push(kvp.Key);
                var type = StatType.ParseType(kvp.Key, out string actualName);
                switch (type)
                {
                    case KvpType.Field:
                        fields.Add(new FieldAssignment(actualName, kvp.Value.ToString()));
                        break;
                    default:
                        ctx.Report($"Expected a field assignment, got {type}.");
                        break;
                }
                ctx.Pop();
            }            
        }
    } 
}