using System.Collections.Generic;
using System.IO;
using Hopper.Meta;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Hopper.Meta.Template;
using System.Linq;

namespace Hopper.Meta.Stats
{
    public struct Metadata
    {
        public string alias;
        public bool isIdentified;
        public Scope<StatType> scope;
    }

    public class StatType
    {
        public string name;
        public string Name => metadata.alias == null ? name : metadata.alias;
        
        public Stack<string> GetScopeQualification()
        {
            var stack = new Stack<string>();
            var scope = metadata.scope;
            while (scope != null)
            {
                stack.Push(scope.name);
                scope = scope.parentScope;
            }
            return stack;
        }
        public string QualifiedName => System.String.Join(".", GetScopeQualification());
        
        public string JoinedParams => System.String.Join(", ", fields.Select(f => $"{f.metadata.type} {f.name}"));

        public bool IsIdentifying => identifiedType != null;
        public bool IsIdentified => metadata.isIdentified;

        public List<FieldDeclaration> fields;
        // TODO: this is kind of dumb, since if one has fields, the other one shall be empty (maybe)
        // I'm leaving this detail for later
        public List<StaticObjectFieldDeclaration> staticIndentiyingFields;
        public List<StatType> nestedTypes;
        public StatType identifiedType;
        public Metadata metadata; 

        public StatType()
        {
            fields = new List<FieldDeclaration>();
            staticIndentiyingFields = new List<StaticObjectFieldDeclaration>();
            nestedTypes = new List<StatType>();
        }

        public static StatType ParseJson(ParsingContext ctx, string inPath)
        {
            string statJson = File.ReadAllText(inPath);
            var jobj = JObject.Parse(statJson);
            
            var statName = Path.GetFileNameWithoutExtension(inPath);
            ctx.ResetFileName(inPath);
            ctx.PushScope(statName);

            var stat = new StatType();
            stat.name = statName;
            stat.metadata.scope = ctx.scope;
            stat.Populate(jobj, ctx);

            ctx.PopScope();

            return stat;
        }

        public void Populate(JObject jobj, ParsingContext ctx)
        {
            foreach (var kvp in jobj)
            {
                ctx.Push(kvp.Key);
                var type = ParseType(kvp.Key, out string actualName);
                switch (type)
                {
                    case KvpType.Field:
                        fields.Add(new FieldDeclaration(actualName, FieldMetadata.Parse(kvp.Value, ctx)));
                        break;
                    case KvpType.Metadata:
                        if (actualName == "identifies") { /*skip*/ }
                        else if (actualName == "alias") { metadata.alias = (string) kvp.Value;    }
                        else { ctx.Report($"Unrecognized metadata: {kvp.Key}"); }
                        break;
                    case KvpType.StaticField:
                        staticIndentiyingFields.Add(new StaticObjectFieldDeclaration(actualName, StaticStatFieldMetadata.Parse(kvp.Value, ctx)));
                        break;
                    case KvpType.NestedType:
                        if (kvp.Value is JObject jobj_nested)
                        {
                            ctx.PushScope(this);
                            var nestedType = new StatType();
                            nestedType.name = actualName;
                            nestedType.Populate(jobj_nested, ctx);
                            nestedType.metadata.scope = ctx.scope;
                            
                            if (jobj.TryGetValue("@identifies", out var token) 
                                && token.Value<string>() == actualName)
                            {
                                identifiedType = nestedType;
                                identifiedType.metadata.isIdentified = true;
                            }
                            nestedTypes.Add(nestedType);
                            ctx.PopScope();
                        }
                        else
                        {
                            ctx.Report($"Nested types must be objects, got {kvp.Value.Type}");
                        }
                        break;
                }
                ctx.Pop();
            }
        }

        public static KvpType ParseType(string name, out string actualName)
        {
            if (name.StartsWith("_")) { actualName = name.Substring(1); return KvpType.NestedType;  }
            if (name.StartsWith("@")) { actualName = name.Substring(1); return KvpType.Metadata;    }
            if (name.StartsWith("$")) { actualName = name.Substring(1); return KvpType.StaticField; }

            actualName = name; return KvpType.Field;
        }

        // F*CK this, it is too dang expressive XD
        /* public void ToCode__()
        {
            var _public = Token(SyntaxKind.PublicKeyword);

            var structDecl = StructDeclaration(name);
            structDecl.AddModifiers(_public);
            structDecl.AddBaseListTypes(SimpleBaseType(ParseTypeName("IStat")));
            
            var constrDecl = ConstructorDeclaration(name);
            var constrParameters = ParameterList();
            var constrBody = Block();

            var addWith = MethodDeclaration(IdentifierName(name), Identifier("AddWith"));
            addWith.AddModifiers(_public);
            addWith.AddParameterListParameters(
                Parameter(Identifier("other"))
                .WithType(IdentifierName(name)));

            var addWithArguments = ArgumentList();

            foreach (var field in fields)
            {
                var fieldType = ParseTypeName(field.metadata.type);
                var fieldName = VariableDeclarator(field.name);
                var fieldVariable = VariableDeclaration(fieldType).AddVariables(fieldName);
                var fieldDecl = FieldDeclaration(fieldVariable)
                    .AddModifiers(_public);
                
                var fieldAsParam = Parameter(Identifier(field.name)).WithType(fieldType);
                var fieldAsArgument = Argument(IdentifierName(field.name));


                if (field.metadata.type == "int")
                {
                    addWithArguments.AddArguments(
                        Argument(ParseExpression($"{field.name} = other.{field.name}")));
                }
                else
                {
                    // sample : sample
                    addWithArguments.AddArguments(
                        fieldAsArgument.WithNameColon(NameColon(IdentifierName(field.name))));
                }

                structDecl.AddMembers(fieldDecl);

                constrBody.AddStatements(ExpressionStatement(
                    AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, 
                        MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                            ThisExpression(), IdentifierName(field.name)),
                            IdentifierName(field.name))));

                constrParameters.AddParameters(fieldAsParam);
            }

            
            var addWithBody = ArrowExpressionClause(
                ObjectCreationExpression(IdentifierName(name))
                .WithArgumentList(addWithArguments));

            constrDecl.WithBody(constrBody).WithParameterList(constrParameters);
            structDecl.AddMembers(constrDecl);
        }
        */

    }
}