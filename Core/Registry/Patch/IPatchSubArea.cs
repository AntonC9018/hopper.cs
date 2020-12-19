namespace Hopper.Core.Registries
{
    // The motivation for this new type of registry is that
    // On initialization entity facctories require some additional data, dependent on the current registry
    // Since I don't want to touch the global state so that the app stays more manageable,
    // I thought, that for at initializtion I would pass the factories the regitry to be used
    // for getting data 

    public interface IPatchSubArea<out T>
    {
        T TryGet(int id);
    }
}