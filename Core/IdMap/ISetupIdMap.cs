using System.Collections.Generic;

namespace Core
{
    public interface ISetupIdMap
    {
        void SetServerMap(List<MapInstruction> instructions);
        List<MapInstruction> PackModMap();
    }
}