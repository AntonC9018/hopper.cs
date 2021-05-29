using System;

namespace Hopper.Shared.Attributes
{
    /// <summary>
    /// Autogenerates 2 shortcut methods as extensions for the entity class. 
    /// The first method tries to retrieve the specified component and call the specified method.
    /// The other one calls it directly.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class AliasAttribute : Attribute
    {
        public string Alias;
        public AliasAttribute(string alias)
        {
            Alias = alias;
        }
    }

    public enum ChainContributionType
    {
        Instance = 0, 
        More = 1, 
        Global = 2, 
    }

    public static class ChainContribution
    {
        public static string GetPrefix(this ChainContributionType type)
        {
            switch (type)
            {
            case ChainContributionType.More:
                return "+";
            case ChainContributionType.Global:
                return "@";
            case ChainContributionType.Instance:
                return "";
            default:
                throw new System.Exception("Never gets to here");
            }
        }

        public static ChainContributionType StripContributionType(ref string uid)
        {
            switch (uid[0])
            {
            case '+':
                uid = uid.Substring(1);
                return ChainContributionType.More;
            case '@':
                uid = uid.Substring(1);
                return ChainContributionType.Global;
            default:
                return ChainContributionType.Instance;
            }
        }
    }

    /// <summary>
    /// Enables autogeneration of fields and methods related to the given chain.
    /// You may mark your instance chains with this attribute in your behavior classes
    /// for a corresponding ChainPath to be autogenerated and to make handlers able to
    /// export for this chain.
    /// You may mark a static index field of your chain type on any class to export to 
    /// MoreChains component, which enables entities to have chains independent 
    /// of their actual components.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class ChainAttribute : Attribute
    {
        public string Name;
        public ChainContributionType Type = ChainContributionType.Instance;

        public ChainAttribute(string name)
        {
            Type = ChainContribution.StripContributionType(ref name);
            Name = name;
        }
    }

    /// <summary>
    /// Indicates the required fields that must be filled in on initialization.
    /// This means that this field will be required in order to instantiate the given
    /// component. The contructor and an AddTo static method are generated automativally,
    /// with or without any of the fields marked with this attribute.
    ///
    /// A constructor will not be generated if the component already has a constructor
    /// that accepts values for all of the injected fields as arguments.
    ///
    /// The autogenerated copy constructor will reference copy any of these fields.
    /// That one may also be overriden by the one provided by user with the same signature. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class InjectAttribute : Attribute {}

    /// <summary>
    /// Enables autogeneration of helper methods for the given enum.
    /// Currently, generates methods HasEitherFlag and HasNeitherFlag.
    /// Also, this attribute inherits form System.Flag, so HasFlag will also become available.
    /// </summary>
    [AttributeUsage(AttributeTargets.Enum)]
    public class FlagsAttribute : System.FlagsAttribute {}


    /// <summary>
    /// Enables automatic generation of the Activate() method.
    /// By default, it would construct the Context, traverse the Check chain, check if 
    /// the check chain succeeded and then traverse the Do chain.
    /// Returns true if the check succeeds.
    /// You cannot define any more custom chains if this attribute is present.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class AutoActivationAttribute : Attribute 
    {
        public string Alias;
        public AutoActivationAttribute(string alias)
        {
            Alias = alias;
        }
    }

    /// <summary>
    /// Marks the method for export.
    ///
    /// Exporting a method in a behavior means autogenerating a static adapter method for it,
    /// which passes along to the method all the necessary data from the context.
    /// The given method becomes available as a handler to json files that define
    /// the default chain structure of behavior presets.  
    ///
    /// In this case the export is considered "static", since it is not supposed to be hooked up
    /// onto a different behavior at runtime, e.g. status effects and item effects.
    ///
    /// This attribute may also be applied to target behaviors outside the class it was used in.
    /// In this case, names of chains have to be provided via Chains.
    /// This feature enables usage of this attribute outside behavior classes.
    ///
    /// You may also mark methods for export on a non-static class, in which case the adapters and
    /// the handlers will be non-static too. For that, see <cref>InstanceExportAttribute</cref>.
    ///
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ExportAttribute : Attribute
    {
        /// <summary>
        /// Which chain to apply the handler to.
        /// Not required if the handler is defined within the behavior class it targets
        /// (the first chain is chosen in this case).
        /// The name of the chain must be of for [+]Behavior.ChainName.
        /// The plus at front means exporting for MoreChains component.
        /// </summary>
        /// <example>
        /// "Attacking.Check" would export for the Check chain of Attacking behavior.
        /// "+MyClass.MyChain" would export for the MyChain chain, contributed to MoreChains component
        /// by the exporting class MyClass.
        /// "@MyClass.MyChain" would export for the MyChain chain, contributed to GlobalChains (world chains)
        /// by the exporting class MyClass.
        /// </example>
        public string Chain = null;

        /// <summary>
        /// This field specifies the priority rank given to the attribute. 
        /// The actual priority number will be given to the handler at initialization stage (through
        /// autogenerated code). Note that these autogenerated priority numbers will be unique.
        /// </summary>
        public PriorityRank Priority = PriorityRank.Default;

        /// <summary>
        /// If this is set to true, a wrapper with apply and remove methods will be generated for this handler.
        /// Note that if you plan to use handlers in a group (aka former retoucher/tinker),
        /// set this to false and create the HandlerGroup yourself.
        /// The value of this field does not affect whether or not the method will be exposed to json.
        /// </summary>
        public bool Dynamic = false;
    }


    /// <summary>
    /// Indicates that the given (non-static!) class / struct wants its methods to be exported.
    /// In this case, InitHandlers() methods, as well as possibly some others, will be generated
    /// as non-static members of the class / struct. The exported handlers, their adapters 
    /// and optionally their wrappers.
    /// This is useful for creating closures for your handlers, e.g. capturing the identifier of
    /// the entity modifier to be removed.
    /// The presense of non-static member functions marked for export on a class without this
    /// attribute will trigger an error (unless the class is a component).
    /// This attribute cannot be applied to components or behaviors (since that would be ambiguous:
    /// do I need to generate handlers for this instance method to be a static member, retrieving
    /// the component of type in which it has been defined from the entity, or do I generate an instance
    /// handler on that component (as a non-static field)?).
    /// The same class may also export static methods, in which case the handlers corresponding to these
    /// will be static too.
    ///
    /// Instances of this class are not going to be detected automatically. 
    /// You need to additionally mark them with this same attribute to indicate export. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field)]
    public class InstanceExportAttribute : Attribute
    {
    }

    /// <summary>
    /// The marked field's Init() method will be called at content initialization stage.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class RequiringInitAttribute : Attribute
    {
    }

    /// <summary>
    /// Indicates that the given field of the Context class should be omitted for AutoActivation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class OmitAttribute : Attribute
    {
    }

    /// <summary>
    /// Marks an entity type for export.
    /// The given class must contain a static EntityFactory and methods.
    /// AddComponents(), InitComponents() and Retouch() that all take in an EntityFactory.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class EntityTypeAttribute : Attribute 
    {
        public bool IsGenerated = false;
        public bool Abstract = false;
    }


    /// <summary>
    /// Marks a slot for export.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class SlotAttribute : Attribute
    {
        public string Name = null;

        public SlotAttribute(string slotName) 
        {
            Name = slotName;
        }
        
        public SlotAttribute() {}
    }

    /// <summary>
    /// Marks an identifying stat field (aka source) for export.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class IdentifyingStatAttribute : Attribute
    {
    }


    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class RegistryAttribute : Attribute
    {
        public string Name = null;

        public RegistryAttribute(string name)
        {
            name = Name;
        }
    }
}