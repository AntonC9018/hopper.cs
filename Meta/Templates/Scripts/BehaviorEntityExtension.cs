using Meta;

namespace Hopper.Meta.Template
{
    public partial class BehaviorEntityExtensions
    {
        public BehaviorSymbolWrapper behavior;
        public BehaviorSymbolWrapper component => behavior;
    }
}