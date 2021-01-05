namespace Hopper.Core.Registries
{
    /** <summary>
        The motivation for this new type of registry is that
        on initialization entity factories require some additional data, dependent on the current registry.
        Since I don't want to touch the global state so that the app stays more manageable,
        I thought, that for at initializtion I would pass the factories the registry to be used
        for getting data.
        </summary>
    */

    public interface IPatchSubArea<out T>
    {
        T TryGet(int id);
    }
}