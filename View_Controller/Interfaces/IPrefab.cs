using System.Collections.Generic;
using Hopper.Utils.Vector;

namespace Hopper.Controller
{
    public interface IModel<out T> where T : ISceneEntity
    {
        T Instantiate(IntVector2 pos, IntVector2 orientation);
        IReadOnlyList<ISieve> Sieves { get; }
    }
}