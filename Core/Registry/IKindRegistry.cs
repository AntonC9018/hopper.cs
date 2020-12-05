using System.Collections.Generic;

namespace Hopper.Core
{
    public interface IKindRegistry
    {
        void SetServerMap(List<MapInstruction> instructions);
        List<MapInstruction> PackModMap();
    }
}