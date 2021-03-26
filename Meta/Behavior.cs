
namespace Hopper.Meta.Template
{
    public class ComponentInfo
    {
        public string Namespace;
        public string ClassName;
        public virtual string TypeText => "component";
    }

    public class TagInfo : ComponentInfo
    {
        public override string TypeText => "tag";
    }
    // A preliminary info class
    // I'll have to add semantical info here as well
    public class BehaviorInfo : ComponentInfo
    {
        public override string TypeText => "behavior";
        public string ActivationAlias;
        public string Params() => "";
        public string ParamNames() => "";
        public bool Check;
    }

    public class ChainInfo
    {
        public string Name; 
        public bool ShouldGenerateParamsMethod(string chainName) => true;
        public bool ShouldGenerateTraverseMethod(string chainName) => true;
    }

    public class ChainsInfo
    {
        public ChainInfo[] ChainInfos;
        public bool ShouldGenerateActivation() => true;
    }

    public class HandlerAdapterInfo
    {
        public string HandlerName;
        public string FirstParam() => "";
        public string ParamNames() => ""; 
    }

    public class PresetInfo
    {
        public string Name;
        public string Handlers() => "";
    }
}