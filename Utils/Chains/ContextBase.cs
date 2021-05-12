namespace Hopper.Utils.Chains
{
    public interface IPropagating
    {
        bool Propagate { get; }
    }
    
    public class ContextBase : IPropagating
    {
        public bool Propagate { get; set; } = true;
    }
}