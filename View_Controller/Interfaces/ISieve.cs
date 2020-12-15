using Hopper.Core.History;

namespace Hopper.Controller
{
    public interface ISieve
    {
        int Weight { get; }
        bool IsFull { get; }
        void Sieve(UpdateCode code);
        void Reset();
    }
}