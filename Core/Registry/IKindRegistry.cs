using System.Collections.Generic;

namespace Core
{
    public interface IKindRegistry
    {
        void SetServerMap(List<MapInstruction> instructions);
        List<MapInstruction> PackModMap();
    }
}