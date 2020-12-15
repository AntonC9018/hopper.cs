using Hopper.Utils.Vector;

namespace Hopper.Controller
{
    public interface ISceneEntity
    {
        void EnterPhase(Hopper.Core.History.EntityState finalState, ISieve sieve, AnimationInfo animationInfo);
        void Update(AnimationInfo animationInfo);
        void SetInitialPosition(Vector2 pos);
        void ChangePos(Vector2 pos);
        void SetInitialOrientation(IntVector2 pos);
        void ChangeOrientation(IntVector2 orientation);
    }
}