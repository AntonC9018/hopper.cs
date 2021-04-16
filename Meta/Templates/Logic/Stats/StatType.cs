using System.Collections.Generic;
using System.IO;
using Hopper.Meta;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Hopper.Meta.Stats
{
    public struct Metadata
    {
        public bool identifies;
        public string alias;
    }

    public struct FieldMetadata
    {
        public string type;
        public string _default;
    }

    public struct Field
    {
        public string name;
        public FieldMetadata metadata;

        public Field(string name, FieldMetadata metadata)
        {
            this.name = name;
            this.metadata = metadata;
        }
    }

    public class StatType
    {
        public static string InPath = @"../Core/Stats/Json/Attack.json";
        public static string OutFolder = @"../Core/Autogen/Stats";
        public static string OutPath = $@"{OutFolder}/Attack.json";
        

        public List<Field> fields;
        public Metadata metadata; 

        public StatType()
        {
            fields = new List<Field>();
        }

        public static void ParseJson()
        {
            string statJson = File.ReadAllText(InPath);
            var obj = JObject.Parse(statJson);
        }

        public enum KvpType
        {
            Field, Metadata, StaticField, NestedType
        }

        public static StatType ParseJObjectAsStatType(JObject jobj)
        {
            var result = new StatType();

            foreach (var kvp in jobj)
            switch (ParseType(kvp.Key, out string actualName))
            {
                case KvpType.Field:
                    result.fields.Add(new Field(actualName, ParseAsField(kvp.Value, actualName)));
                    break;
                case KvpType.Metadata:
                    if (actualName == "identifies") { result.metadata.identifies = (bool) kvp.Value; }
                    else if (actualName == "alias") { result.metadata.alias = (string) kvp.Value;    }
                    break;
                case KvpType.StaticField:

                    break;
                case KvpType.NestedType:
                    break;
            }
        }

        public static FieldMetadata ParseAsField(JToken jtok, string name)
        {
            if (jtok is JValue jval)
            {
                if (jval.Type == JTokenType.Integer)
                {
                    return new FieldMetadata 
                    { 
                        type = "int",
                        _default = jval.Value.ToString()
                    };
                }
            }
            else if (jtok is JObject jobj)
            {
                var result = new FieldMetadata();

                foreach (var kvp in jobj)
                {
                    var type = ParseType(kvp.Key, out string actualName);
                    switch (type)
                    {
                        case KvpType.Metadata:
                            if (actualName == "type") { result.type = kvp.Value.ToString(); }
                            else if (actualName == "default") 
                            { 
                                if (kvp.Value.Type == JTokenType.String)
                                {
                                    result._default = (string) kvp.Value.ToString(); 
                                }
                                else
                                {

                                }
                            }
                            else throw new SyntaxException($"Unexpected metadata name {kvp.Key} while parsing {name}. Expected either @type or @default");
                            break;
                        default:
                            throw new SyntaxException($"Parsed a {type} with name {kvp.Key}. Expected a Metadata (the name must start with a @).");
                    }
                }

                return result;
            }
            throw new SyntaxException($"While parsing {name}, expected an object or an int, received {jtok.GetType()}.");
        }

        public static KvpType ParseType(string name, out string actualName)
        {
            if (name.StartsWith("_")) { actualName = name.Substring(1); return KvpType.NestedType;  }
            if (name.StartsWith("@")) { actualName = name.Substring(1); return KvpType.Metadata;    }
            if (name.StartsWith("$")) { actualName = name.Substring(1); return KvpType.StaticField; }

            actualName = name; return KvpType.Field;
        }

    }
}