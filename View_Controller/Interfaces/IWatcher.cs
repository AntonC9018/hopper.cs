using Hopper.Core;

namespace Hopper.Controller
{
    public interface IWatcher
    {
        void Watch(World world, ViewController vm);
    }
}