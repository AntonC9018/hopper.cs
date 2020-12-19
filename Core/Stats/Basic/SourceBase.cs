using Hopper.Core.Registries;

namespace Hopper.Core.Stats.Basic
{
    public class SourceBase<T> : Extendent<T> where T : SourceBase<T>
    {
        public int resistance;
    }
}