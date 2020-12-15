using System.Collections.Generic;

namespace Hopper.Controller
{
    public class HistoryData
    {
        public EntityStatesAndSieves entityStatesAndSieves;
        public ISceneEntity sceneEnt;
    }

    public interface IAnimator
    {
        void Animate(IEnumerable<HistoryData> historyData);
        void SetCamera(CameraState cameraInitialPosition);
    }
}
