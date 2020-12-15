using Hopper.Utils.Vector;

namespace Hopper.Controller
{
    public interface ICamera
    {
        void SetInitialPosition(Vector2 pos);
        void ChangePos(Vector2 pos);
    }
}