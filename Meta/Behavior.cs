
namespace Hopper.Meta.Template
{
    // A preliminary info class
    // I'll have to add semantical info here as well
    public class BehaviorInfo
    {
        public string ClassName;
        public string Namespace;
        public string ActivationAlias;
        public string Params() => "";
        public string ParamNames() => "";
        public bool Check;
    }

    public class ContextInfo
    {
        public string Params() => "";
        public string ParamsNames() => "";
        public string ParamInitialization() => "";
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
        public bool CallsMethodOnFirstParam() => true;
        public string FirstParam() => "";
        public string ParamNames() => ""; 
    }

    public class PresetInfo
    {
        public string Name;
        public string Handlers() => "";
    }
}