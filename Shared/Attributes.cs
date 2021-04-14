using System;

namespace Hopper.Shared.Attributes
{
    /// <summary>
    /// Enables autogeneration of various shortcuts for acessing methods of the
    /// behavior through the entity. They are added as extension methods over entities.
    /// The parameter passed down the constructor indicates the name of those methods.
    /// The resulting code depends on the chains that the behavior adds.
    /// </summary>
    /// <example>
    /// Applying the attribute with alias "Attack" and chains "Do" and "Check"
    /// will create the following extension methods:
    /// Act(), TryAct()                            methods call the activation
    /// UnconditionalAct(), TryUnconditionalAct()  call the Do() method
    /// CheckAct(), TryCheckAct()                  methods call the Check() method 
    /// </example>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class ActivationAliasAttribute : Attribute
    {
        public ActivationAliasAttribute(string alias)
        {
        }
    }

    /// <summary>
    /// Autogenerates 2 shortcut methods as extensions for the entity class. 
    /// The first method tries to retrieve the specified component and call the specified method.
    /// The other one calls it directly.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class AliasAttribute : Attribute
    {
        public AliasAttribute(string alias)
        {
        }
    }

    /// <summary>
    /// Enables autogeneration of chain related fields and methods of the behavior.
    /// Important: you must define the Context type that will be passed down 
    /// the chain in the scope of your behavior class. The autogenerated chains
    /// will work off of this Context type. 
    /// </summary>
    /// <example>
    /// Say, the selected chains are "Check" and "Do". 
    /// Fields for the chain objects will be autogenerated.
    /// The generated code will contain methods Check() and Do(),
    /// which contruct the context by filling in 
    /// the individual fields and calls the other method pair.
    /// The other pair is TraverseCheck() and TraverseDo() which traverse 
    /// the chain with the given context and returns context.success by default.
    /// If a custom overload for one of the signatures has been provided, 
    /// the corresponing overload will not be autogenerated.
    /// </example>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class ChainsAttribute : Attribute
    {
        public ChainsAttribute(params string[] chains)
        {
        }
    }

    /// <summary>
    /// Indicates the required fields that must be filled in on initialization 
    /// of the given Behavior. *Some init method stuff, don't know yet*
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class InjectAttribute : Attribute {}

    /// <summary>
    /// !!! This feature has not been implemented !!!
    /// Enables autogeneration of helper methods for the given enum.
    /// If the scope is a namespace or a class, the generated code would
    /// contain just extension methods for the enum type, in a static class.
    /// If the attribute is applied to a field of an enum type, the methods for
    /// querying that field will be created. It is required for the enum itself
    /// to have been applied this attribute too.
    /// E.g. an enum with members `Nice` and `Cool` 
    /// in an enum named Hello will generate the following methods:
    /// if in a namespace or a class scope:   IsNice(this Hello), IsCool(this Hello),
    ///             Hello SetNice(this Hello, bool), Hello SetCool(this Hello, bool);
    /// if applied to a field:                IsNice(), IsCool(), ClearHello(),
    ///                                       SetNice(bool), SetCool(bool).
    /// May also be used in combination with 
    /// </summary>
    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field)]
    public class FlagsAttribute : System.FlagsAttribute {}


    /// <summary>
    /// Enables automatic generation of the Activate() method.
    /// By default, it would construct the Context, traverse the Check chain, check if 
    /// the check chain succeeded and the traverse the Do chain.
    /// Returns true if the check succeeds.
    /// If this attribute is provided and there is no Chains attribute, the class
    /// is assumed to have Chains("Check", "Do") attribute applied instead.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class AutoActivationAttribute : Attribute 
    {
        public AutoActivationAttribute(string alias)
        {
        }
    }

    /// <summary>
    /// Marks the method for export.
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
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ExportAttribute : Attribute
    {
        // e.g. "Attackable.Do"
        public string Chain = null;

        /// <summary>
        /// This field specifies the priority rank given to the attribute. 
        /// The actual priority number will be given to the handler at initialization stage (through
        /// autogenerated code). Note that these autogenerated priority numbers will be unique.
        /// </summary>
        public PriorityRank Priority = PriorityRank.Default;

        /// <summary>
        /// If this is set to true, a wrapper with apply and remove methods will be generated for this chain.
        /// Note that if you plan to use handlers in a group (aka former retoucher/tinker),
        /// set this to false and create the HandlerGroup yourself.
        /// The value of this field does not affect whether or not the method will be exposed to json.
        /// </summary>
        public bool Dynamic = false;
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class OmitAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class EntityTypeAttribute : Attribute 
    {
        bool IsGenerated = false;
    }
}