
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
        public string initializationParams
    }

    public class ChainsInfo
    {
        public BehaviorInfo behavior;
        public string ctxInitializationParams;
        public string ctxNames;
    }
}