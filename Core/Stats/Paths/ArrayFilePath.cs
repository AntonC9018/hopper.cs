namespace Hopper.Core.Stats.Basic
{
    // These are always created per kind, which means they will 
    // have a reference to Registry once used in runtime or the patching stage
    // as opposed to the initialization stage.
    // public class ArrayFilePath<T> : StatPath<ArrayFile>
    //     where T : WithResValue, IPatch
    // {
    //     public ArrayFilePath(string path, Resistance_Kind<T> resistance)
    //         : base(path, GetDefaultFile(resistance))
    //     {
    //     }


    // }
}