using System;
using Core.Behaviors;

namespace Core
{
    public class Flavor
    {
        public int amount;
        public int power;
        public int source;

        public virtual int Decrement()
        {
            amount--;
            return amount;
        }
    }

    public interface IFlavorTinkerData
    {
        public Flavor Flavor { get; set; }
    }

    public interface IStatus
    {
        public void Apply(Entity entity, Flavor f);
        public int Decrement(Entity entity);
    }

    public class Status<T> : IStatus
        where T : TinkerData, IFlavorTinkerData, new()
    {
        Tinker<T> tinker;
        public void Apply(Entity entity, Flavor f)
        {
            entity.TinkAndSave(tinker);
            var store = tinker.GetStore(entity.id);
            store.Flavor = f;
        }
        public int Decrement(Entity entity)
        {
            var store = tinker.GetStore(entity.id);
            return store.Flavor.Decrement();
        }
    }
}