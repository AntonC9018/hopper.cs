//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Hopper.Meta.Template {
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;
    using System.IO;
    using System;
    
    
    public partial class BehaviorEntityExtensions : IBehaviorEntityExtensionsBase {
        
        private global::Microsoft.VisualStudio.TextTemplating.ITextTemplatingEngineHost hostValue;
        
        
        private Hopper.Meta.Template.BehaviorInfo _behaviorField;
        
        public Hopper.Meta.Template.BehaviorInfo behavior {
            get {
                return this._behaviorField;
            }
        }

        
        public global::Microsoft.VisualStudio.TextTemplating.ITextTemplatingEngineHost Host {
            get {
                return this.hostValue;
            }
            set {
                this.hostValue = value;
            }
        }
        
        public virtual string TransformText() {
            this.GenerationEnvironment = null;
            
            #line 9 "Templates\BehaviorEntityExtensions.tt"
            this.Write("\r\n    /// <summary>\r\n    /// @Autogenerated\r\n    /// This is a shortcut method.\r\n" +
                    "    /// Queries the ");
            
            #line default
            #line hidden
            
            #line 13 "Templates\BehaviorEntityExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture( behavior.ClassName ));
            
            #line default
            #line hidden
            
            #line 13 "Templates\BehaviorEntityExtensions.tt"
            this.Write(" behavior on the entity.\r\n    /// Use TryGet");
            
            #line default
            #line hidden
            
            #line 14 "Templates\BehaviorEntityExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture( behavior.ClassName ));
            
            #line default
            #line hidden
            
            #line 14 "Templates\BehaviorEntityExtensions.tt"
            this.Write("() if you\'re not sure whether the entity has the given behavior.\r\n    /// </summa" +
                    "ry>\r\n    public static ");
            
            #line default
            #line hidden
            
            #line 16 "Templates\BehaviorEntityExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture( behavior.ClassName ));
            
            #line default
            #line hidden
            
            #line 16 "Templates\BehaviorEntityExtensions.tt"
            this.Write(" Get");
            
            #line default
            #line hidden
            
            #line 16 "Templates\BehaviorEntityExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture( behavior.ClassName ));
            
            #line default
            #line hidden
            
            #line 16 "Templates\BehaviorEntityExtensions.tt"
            this.Write("(this Entity entity)\r\n    {\r\n        return entity.GetBehavior(");
            
            #line default
            #line hidden
            
            #line 18 "Templates\BehaviorEntityExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture( behavior.ClassName ));
            
            #line default
            #line hidden
            
            #line 18 "Templates\BehaviorEntityExtensions.tt"
            this.Write(".Index);\r\n    }\r\n\r\n    /// <summary>\r\n    /// @Autogenerated\r\n    /// This is a s" +
                    "hortcut method.\r\n    /// Check if the entity has the ");
            
            #line default
            #line hidden
            
            #line 24 "Templates\BehaviorEntityExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture( behavior.ClassName ));
            
            #line default
            #line hidden
            
            #line 24 "Templates\BehaviorEntityExtensions.tt"
            this.Write(" behavior.\r\n    /// </summary>\r\n    public static bool Has");
            
            #line default
            #line hidden
            
            #line 26 "Templates\BehaviorEntityExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture( behavior.ClassName ));
            
            #line default
            #line hidden
            
            #line 26 "Templates\BehaviorEntityExtensions.tt"
            this.Write("(this Entity entity)\r\n    {\r\n        return entity.HasBehavior(");
            
            #line default
            #line hidden
            
            #line 28 "Templates\BehaviorEntityExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture( behavior.ClassName ));
            
            #line default
            #line hidden
            
            #line 28 "Templates\BehaviorEntityExtensions.tt"
            this.Write(".Index);\r\n    }\r\n    \r\n    /// <summary>\r\n    /// @Autogenerated\r\n    /// This is" +
                    " a shortcut method.\r\n    /// Returns the ");
            
            #line default
            #line hidden
            
            #line 34 "Templates\BehaviorEntityExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture( behavior.ClassName ));
            
            #line default
            #line hidden
            
            #line 34 "Templates\BehaviorEntityExtensions.tt"
            this.Write(" behavior if the entity has it, otherwise returns null.\r\n    /// </summary>\r\n    " +
                    "public static ");
            
            #line default
            #line hidden
            
            #line 36 "Templates\BehaviorEntityExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture( behavior.ClassName ));
            
            #line default
            #line hidden
            
            #line 36 "Templates\BehaviorEntityExtensions.tt"
            this.Write(" TryGet");
            
            #line default
            #line hidden
            
            #line 36 "Templates\BehaviorEntityExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture( behavior.ClassName ));
            
            #line default
            #line hidden
            
            #line 36 "Templates\BehaviorEntityExtensions.tt"
            this.Write("(this Entity entity)\r\n    {\r\n        return entity.TryGetBehavior(");
            
            #line default
            #line hidden
            
            #line 38 "Templates\BehaviorEntityExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture( behavior.ClassName ));
            
            #line default
            #line hidden
            
            #line 38 "Templates\BehaviorEntityExtensions.tt"
            this.Write(".Index);\r\n    }\r\n\r\n    /// <summary>\r\n    /// @Autogenerated\r\n    /// This is a s" +
                    "hortcut method.\r\n    /// Returns the ");
            
            #line default
            #line hidden
            
            #line 44 "Templates\BehaviorEntityExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture( behavior.ClassName ));
            
            #line default
            #line hidden
            
            #line 44 "Templates\BehaviorEntityExtensions.tt"
            this.Write(@" behavior through the out param 
    /// if the entity has it, otherwise returns null.
    /// This method is especially useful in cases where you need to conditionally do something with the behavior.
    /// <summary>
    /// @Autogenerated
    /// </summary>
    /// </summary>
    public static bool TryGet");
            
            #line default
            #line hidden
            
            #line 51 "Templates\BehaviorEntityExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture( behavior.ClassName ));
            
            #line default
            #line hidden
            
            #line 51 "Templates\BehaviorEntityExtensions.tt"
            this.Write("(this Entity entity, out ");
            
            #line default
            #line hidden
            
            #line 51 "Templates\BehaviorEntityExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture( behavior.ClassName ));
            
            #line default
            #line hidden
            
            #line 51 "Templates\BehaviorEntityExtensions.tt"
            this.Write(" behavior)\r\n    {\r\n        behavior = entity.TryGetBehavior(");
            
            #line default
            #line hidden
            
            #line 53 "Templates\BehaviorEntityExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture( behavior.ClassName ));
            
            #line default
            #line hidden
            
            #line 53 "Templates\BehaviorEntityExtensions.tt"
            this.Write(".Index);\r\n        return behavior == null;\r\n    }\r\n\r\n");
            
            #line default
            #line hidden
            
            #line 57 "Templates\BehaviorEntityExtensions.tt"
 
if (behavior.ActivationAlias != null) 
{
    if (behavior.Check)
    {

            
            #line default
            #line hidden
            
            #line 63 "Templates\BehaviorEntityExtensions.tt"
            this.Write("    /// <summary>\r\n    /// @Autogenerated\r\n    /// This is a shortcut method.\r\n  " +
                    "  /// Calls ");
            
            #line default
            #line hidden
            
            #line 66 "Templates\BehaviorEntityExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture( behavior.ClassName ));
            
            #line default
            #line hidden
            
            #line 66 "Templates\BehaviorEntityExtensions.tt"
            this.Write("\'s Check() method. \r\n    /// Returns true if the check succeeds.\r\n    /// The ent" +
                    "ity must have the specified behavior. \r\n    /// Use TryCheck");
            
            #line default
            #line hidden
            
            #line 69 "Templates\BehaviorEntityExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture( behavior.ActivationAlias ));
            
            #line default
            #line hidden
            
            #line 69 "Templates\BehaviorEntityExtensions.tt"
            this.Write("() to first check if the behavior exists.\r\n    /// </summary>\r\n    public static " +
                    "bool Check");
            
            #line default
            #line hidden
            
            #line 71 "Templates\BehaviorEntityExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture( behavior.ActivationAlias ));
            
            #line default
            #line hidden
            
            #line 71 "Templates\BehaviorEntityExtensions.tt"
            this.Write("(this Entity entity");
            
            #line default
            #line hidden
            
            #line 71 "Templates\BehaviorEntityExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture( behavior.Params() ));
            
            #line default
            #line hidden
            
            #line 71 "Templates\BehaviorEntityExtensions.tt"
            this.Write(")\r\n    {\r\n        var behavior = Get");
            
            #line default
            #line hidden
            
            #line 73 "Templates\BehaviorEntityExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture( behavior.ClassName ));
            
            #line default
            #line hidden
            
            #line 73 "Templates\BehaviorEntityExtensions.tt"
            this.Write("(entity);\r\n        return behavior.Check(entity");
            
            #line default
            #line hidden
            
            #line 74 "Templates\BehaviorEntityExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture( behavior.ParamNames() ));
            
            #line default
            #line hidden
            
            #line 74 "Templates\BehaviorEntityExtensions.tt"
            this.Write(");\r\n    }\r\n\r\n    /// <summary>\r\n    /// @Autogenerated\r\n    /// This is a shortcu" +
                    "t method.\r\n    /// Calls ");
            
            #line default
            #line hidden
            
            #line 80 "Templates\BehaviorEntityExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture( behavior.ClassName ));
            
            #line default
            #line hidden
            
            #line 80 "Templates\BehaviorEntityExtensions.tt"
            this.Write("\'s Check() method. \r\n    /// Returns false if the entity does not have specified " +
                    "behavior. \r\n    /// Returns true if it does and the check succeeds.\r\n    /// Use" +
                    " Check");
            
            #line default
            #line hidden
            
            #line 83 "Templates\BehaviorEntityExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture( behavior.ActivationAlias ));
            
            #line default
            #line hidden
            
            #line 83 "Templates\BehaviorEntityExtensions.tt"
            this.Write("() if you know for sure the entity \r\n    /// would have the specified behavior.\r\n" +
                    "    /// </summary>\r\n    public static bool TryCheck");
            
            #line default
            #line hidden
            
            #line 86 "Templates\BehaviorEntityExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture( behavior.ActivationAlias ));
            
            #line default
            #line hidden
            
            #line 86 "Templates\BehaviorEntityExtensions.tt"
            this.Write("(this Entity entity");
            
            #line default
            #line hidden
            
            #line 86 "Templates\BehaviorEntityExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture( behavior.Params() ));
            
            #line default
            #line hidden
            
            #line 86 "Templates\BehaviorEntityExtensions.tt"
            this.Write(")\r\n    {\r\n        var behavior = Get");
            
            #line default
            #line hidden
            
            #line 88 "Templates\BehaviorEntityExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture( behavior.ClassName ));
            
            #line default
            #line hidden
            
            #line 88 "Templates\BehaviorEntityExtensions.tt"
            this.Write("(entity);\r\n        return behavior.Check(entity");
            
            #line default
            #line hidden
            
            #line 89 "Templates\BehaviorEntityExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture( behavior.ParamNames() ));
            
            #line default
            #line hidden
            
            #line 89 "Templates\BehaviorEntityExtensions.tt"
            this.Write(");\r\n    }\r\n\r\n    /// <summary>\r\n    /// @Autogenerated\r\n    /// This is a shortcu" +
                    "t method.\r\n    /// Calls ");
            
            #line default
            #line hidden
            
            #line 95 "Templates\BehaviorEntityExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture( behavior.ClassName ));
            
            #line default
            #line hidden
            
            #line 95 "Templates\BehaviorEntityExtensions.tt"
            this.Write("\'s Do() method, that is, activates the behavior, \r\n    /// without calling the Ch" +
                    "eck() method.\r\n    /// Use this method to force some behavior to activate.\r\n    " +
                    "/// </summary>\r\n    public static void Unconditional");
            
            #line default
            #line hidden
            
            #line 99 "Templates\BehaviorEntityExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture( behavior.ActivationAlias ));
            
            #line default
            #line hidden
            
            #line 99 "Templates\BehaviorEntityExtensions.tt"
            this.Write("(this Entity entity");
            
            #line default
            #line hidden
            
            #line 99 "Templates\BehaviorEntityExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture( behavior.Params() ));
            
            #line default
            #line hidden
            
            #line 99 "Templates\BehaviorEntityExtensions.tt"
            this.Write(")\r\n    {\r\n        var behavior = Get");
            
            #line default
            #line hidden
            
            #line 101 "Templates\BehaviorEntityExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture( behavior.ClassName ));
            
            #line default
            #line hidden
            
            #line 101 "Templates\BehaviorEntityExtensions.tt"
            this.Write("(entity);\r\n        behavior.Do(entity");
            
            #line default
            #line hidden
            
            #line 102 "Templates\BehaviorEntityExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture( behavior.ParamNames() ));
            
            #line default
            #line hidden
            
            #line 102 "Templates\BehaviorEntityExtensions.tt"
            this.Write(");\r\n    }\r\n\r\n    /// <summary>\r\n    /// @Autogenerated\r\n    /// This is a shortcu" +
                    "t method.\r\n    /// Calls ");
            
            #line default
            #line hidden
            
            #line 108 "Templates\BehaviorEntityExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture( behavior.ClassName ));
            
            #line default
            #line hidden
            
            #line 108 "Templates\BehaviorEntityExtensions.tt"
            this.Write("\'s Do() method, that is, activates the behavior, \r\n    /// without calling the Ch" +
                    "eck() method.\r\n    /// Use this method to force some behavior to activate.\r\n    " +
                    "/// Returns false if the ");
            
            #line default
            #line hidden
            
            #line 111 "Templates\BehaviorEntityExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture( behavior.ClassName ));
            
            #line default
            #line hidden
            
            #line 111 "Templates\BehaviorEntityExtensions.tt"
            this.Write(" behavior was not found on entity,\r\n    /// otherwise it indicates the result of " +
                    "the activation.\r\n    /// </summary>\r\n    public static bool TryUnconditional");
            
            #line default
            #line hidden
            
            #line 114 "Templates\BehaviorEntityExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture( behavior.ActivationAlias ));
            
            #line default
            #line hidden
            
            #line 114 "Templates\BehaviorEntityExtensions.tt"
            this.Write("(this Entity entity");
            
            #line default
            #line hidden
            
            #line 114 "Templates\BehaviorEntityExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture( behavior.Params() ));
            
            #line default
            #line hidden
            
            #line 114 "Templates\BehaviorEntityExtensions.tt"
            this.Write(")\r\n    {\r\n        if TryGet");
            
            #line default
            #line hidden
            
            #line 116 "Templates\BehaviorEntityExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture( behavior.ClassName ));
            
            #line default
            #line hidden
            
            #line 116 "Templates\BehaviorEntityExtensions.tt"
            this.Write("(entity, out var behavior)\r\n        {\r\n            return behavior.Do(entity");
            
            #line default
            #line hidden
            
            #line 118 "Templates\BehaviorEntityExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture( behavior.ParamNames() ));
            
            #line default
            #line hidden
            
            #line 118 "Templates\BehaviorEntityExtensions.tt"
            this.Write(");\r\n        }\r\n        return false;\r\n    }\r\n\r\n");
            
            #line default
            #line hidden
            
            #line 123 "Templates\BehaviorEntityExtensions.tt"
  
    }

            
            #line default
            #line hidden
            
            #line 126 "Templates\BehaviorEntityExtensions.tt"
            this.Write("    /// <summary>\r\n    /// @Autogenerated\r\n    /// This is a shortcut method.\r\n  " +
                    "  /// Queries the ");
            
            #line default
            #line hidden
            
            #line 129 "Templates\BehaviorEntityExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture( behavior.ClassName ));
            
            #line default
            #line hidden
            
            #line 129 "Templates\BehaviorEntityExtensions.tt"
            this.Write(" behavior on the entity and activates it.\r\n    /// This method should be used if " +
                    "you are sure the entity has the ");
            
            #line default
            #line hidden
            
            #line 130 "Templates\BehaviorEntityExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture( behavior.ClassName ));
            
            #line default
            #line hidden
            
            #line 130 "Templates\BehaviorEntityExtensions.tt"
            this.Write(" behavior.\r\n    /// If you\'re not sure whether it has the ");
            
            #line default
            #line hidden
            
            #line 131 "Templates\BehaviorEntityExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture( behavior.ClassName ));
            
            #line default
            #line hidden
            
            #line 131 "Templates\BehaviorEntityExtensions.tt"
            this.Write(" behavior, \r\n    /// use Try");
            
            #line default
            #line hidden
            
            #line 132 "Templates\BehaviorEntityExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture( behavior.ActivationAlias ));
            
            #line default
            #line hidden
            
            #line 132 "Templates\BehaviorEntityExtensions.tt"
            this.Write("() method instead.\r\n    /// </summary>\r\n    public static bool ");
            
            #line default
            #line hidden
            
            #line 134 "Templates\BehaviorEntityExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture( behavior.ActivationAlias ));
            
            #line default
            #line hidden
            
            #line 134 "Templates\BehaviorEntityExtensions.tt"
            this.Write("(this Entity entity");
            
            #line default
            #line hidden
            
            #line 134 "Templates\BehaviorEntityExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture( behavior.Params() ));
            
            #line default
            #line hidden
            
            #line 134 "Templates\BehaviorEntityExtensions.tt"
            this.Write(")\r\n    {\r\n        var behavior = Get");
            
            #line default
            #line hidden
            
            #line 136 "Templates\BehaviorEntityExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture( behavior.ClassName ));
            
            #line default
            #line hidden
            
            #line 136 "Templates\BehaviorEntityExtensions.tt"
            this.Write("(entity);\r\n        return behavior.Activate(entity");
            
            #line default
            #line hidden
            
            #line 137 "Templates\BehaviorEntityExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture( behavior.ParamNames() ));
            
            #line default
            #line hidden
            
            #line 137 "Templates\BehaviorEntityExtensions.tt"
            this.Write(");\r\n    }\r\n\r\n    /// <summary>\r\n    /// @Autogenerated\r\n    /// This is a shortcu" +
                    "t method.\r\n    /// Queries the ");
            
            #line default
            #line hidden
            
            #line 143 "Templates\BehaviorEntityExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture( behavior.ClassName ));
            
            #line default
            #line hidden
            
            #line 143 "Templates\BehaviorEntityExtensions.tt"
            this.Write(" behavior on the entity and activates it.\r\n    /// This method should be used if " +
                    "you are not sure \r\n    /// whether the entity has the ");
            
            #line default
            #line hidden
            
            #line 145 "Templates\BehaviorEntityExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture( behavior.ClassName ));
            
            #line default
            #line hidden
            
            #line 145 "Templates\BehaviorEntityExtensions.tt"
            this.Write(" behavior.\r\n    /// This method returns false if the behavior was not found on th" +
                    "e entity\r\n    /// otherwise it indicates whether the activation succeeds.\r\n    /" +
                    "// </summary>\r\n    public static bool Try");
            
            #line default
            #line hidden
            
            #line 149 "Templates\BehaviorEntityExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture( behavior.ActivationAlias ));
            
            #line default
            #line hidden
            
            #line 149 "Templates\BehaviorEntityExtensions.tt"
            this.Write("(this Entity entity");
            
            #line default
            #line hidden
            
            #line 149 "Templates\BehaviorEntityExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture( behavior.Params() ));
            
            #line default
            #line hidden
            
            #line 149 "Templates\BehaviorEntityExtensions.tt"
            this.Write(")\r\n    {\r\n        if (TryGet");
            
            #line default
            #line hidden
            
            #line 151 "Templates\BehaviorEntityExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture( behavior.ClassName ));
            
            #line default
            #line hidden
            
            #line 151 "Templates\BehaviorEntityExtensions.tt"
            this.Write("(entity, out var behavior))\r\n        {\r\n            return behavior.Activate(enti" +
                    "ty");
            
            #line default
            #line hidden
            
            #line 153 "Templates\BehaviorEntityExtensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture( behavior.Params()));
            
            #line default
            #line hidden
            
            #line 153 "Templates\BehaviorEntityExtensions.tt"
            this.Write(");\r\n        }\r\n        return false;\r\n    }\r\n");
            
            #line default
            #line hidden
            
            #line 157 "Templates\BehaviorEntityExtensions.tt"
 } 
            
            #line default
            #line hidden
            return this.GenerationEnvironment.ToString();
        }
        
        public virtual void Initialize() {
            if ((this.Host != null)) {
                this.Host.SetFileExtension(".cs");
            }
            if ((this.Errors.HasErrors == false)) {
                bool _behaviorAcquired = false;
                if (((this.Session != null) 
                            && this.Session.ContainsKey("behavior"))) {
                    object data = this.Session["behavior"];
                    if (typeof(Hopper.Meta.Template.BehaviorInfo).IsAssignableFrom(data.GetType())) {
                        this._behaviorField = ((Hopper.Meta.Template.BehaviorInfo)(data));
                        _behaviorAcquired = true;
                    }
                    else {
                        this.Error("The type \'Hopper.Meta.Template.BehaviorInfo\' of the parameter \'behavior\' did not " +
                                "match the type passed to the template");
                    }
                }
                if (((_behaviorAcquired == false) 
                            && (this.Host != null))) {
                    string data = ((Microsoft.VisualStudio.TextTemplating.ITextTemplatingEngineHost)(this.Host)).ResolveParameterValue(null, null, "behavior");
                    if ((data != null)) {
                        System.ComponentModel.TypeConverter dataTypeConverter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(Hopper.Meta.Template.BehaviorInfo));
                        if (((dataTypeConverter != null) 
                                    && dataTypeConverter.CanConvertFrom(typeof(string)))) {
                            this._behaviorField = ((Hopper.Meta.Template.BehaviorInfo)(dataTypeConverter.ConvertFromString(data)));
                        }
                        else {
                            this.Error("The host parameter \'behavior\' could not be converted to the type \'Hopper.Meta.Tem" +
                                    "plate.BehaviorInfo\' specified in the template");
                        }
                    }
                }
            }

        }
    }
    
    public class BehaviorEntityExtensionsBase {
        
        private global::System.Text.StringBuilder builder;
        
        private global::System.Collections.Generic.IDictionary<string, object> session;
        
        private global::System.CodeDom.Compiler.CompilerErrorCollection errors;
        
        private string currentIndent = string.Empty;
        
        private global::System.Collections.Generic.Stack<int> indents;
        
        private ToStringInstanceHelper _toStringHelper = new ToStringInstanceHelper();
        
        public virtual global::System.Collections.Generic.IDictionary<string, object> Session {
            get {
                return this.session;
            }
            set {
                this.session = value;
            }
        }
        
        public global::System.Text.StringBuilder GenerationEnvironment {
            get {
                if ((this.builder == null)) {
                    this.builder = new global::System.Text.StringBuilder();
                }
                return this.builder;
            }
            set {
                this.builder = value;
            }
        }
        
        protected global::System.CodeDom.Compiler.CompilerErrorCollection Errors {
            get {
                if ((this.errors == null)) {
                    this.errors = new global::System.CodeDom.Compiler.CompilerErrorCollection();
                }
                return this.errors;
            }
        }
        
        public string CurrentIndent {
            get {
                return this.currentIndent;
            }
        }
        
        private global::System.Collections.Generic.Stack<int> Indents {
            get {
                if ((this.indents == null)) {
                    this.indents = new global::System.Collections.Generic.Stack<int>();
                }
                return this.indents;
            }
        }
        
        public ToStringInstanceHelper ToStringHelper {
            get {
                return this._toStringHelper;
            }
        }
        
        public void Error(string message) {
            this.Errors.Add(new global::System.CodeDom.Compiler.CompilerError(null, -1, -1, null, message));
        }
        
        public void Warning(string message) {
            global::System.CodeDom.Compiler.CompilerError val = new global::System.CodeDom.Compiler.CompilerError(null, -1, -1, null, message);
            val.IsWarning = true;
            this.Errors.Add(val);
        }
        
        public string PopIndent() {
            if ((this.Indents.Count == 0)) {
                return string.Empty;
            }
            int lastPos = (this.currentIndent.Length - this.Indents.Pop());
            string last = this.currentIndent.Substring(lastPos);
            this.currentIndent = this.currentIndent.Substring(0, lastPos);
            return last;
        }
        
        public void PushIndent(string indent) {
            this.Indents.Push(indent.Length);
            this.currentIndent = (this.currentIndent + indent);
        }
        
        public void ClearIndent() {
            this.currentIndent = string.Empty;
            this.Indents.Clear();
        }
        
        public void Write(string textToAppend) {
            this.GenerationEnvironment.Append(textToAppend);
        }
        
        public void Write(string format, params object[] args) {
            this.GenerationEnvironment.AppendFormat(format, args);
        }
        
        public void WriteLine(string textToAppend) {
            this.GenerationEnvironment.Append(this.currentIndent);
            this.GenerationEnvironment.AppendLine(textToAppend);
        }
        
        public void WriteLine(string format, params object[] args) {
            this.GenerationEnvironment.Append(this.currentIndent);
            this.GenerationEnvironment.AppendFormat(format, args);
            this.GenerationEnvironment.AppendLine();
        }
        
        public class ToStringInstanceHelper {
            
            private global::System.IFormatProvider formatProvider = global::System.Globalization.CultureInfo.InvariantCulture;
            
            public global::System.IFormatProvider FormatProvider {
                get {
                    return this.formatProvider;
                }
                set {
                    if ((value != null)) {
                        this.formatProvider = value;
                    }
                }
            }
            
            public string ToStringWithCulture(object objectToConvert) {
                if ((objectToConvert == null)) {
                    throw new global::System.ArgumentNullException("objectToConvert");
                }
                global::System.Type type = objectToConvert.GetType();
                global::System.Type iConvertibleType = typeof(global::System.IConvertible);
                if (iConvertibleType.IsAssignableFrom(type)) {
                    return ((global::System.IConvertible)(objectToConvert)).ToString(this.formatProvider);
                }
                global::System.Reflection.MethodInfo methInfo = type.GetMethod("ToString", new global::System.Type[] {
                            iConvertibleType});
                if ((methInfo != null)) {
                    return ((string)(methInfo.Invoke(objectToConvert, new object[] {
                                this.formatProvider})));
                }
                return objectToConvert.ToString();
            }
        }
    }
}
