using System;
using Core.Behaviors;

namespace Core
{
    // Statused -> status.Apply(entity, flavor)
    // -> entity.TinkAndSave(tinker)
    // +  tinkerData.flavor = flavor
    // now to see if a status is active one needs to
    // 1. get the tinker's id
    public class Flavor
    {
        public int amount;
        public int power;
        public int source;
    }


    // such tinker data is used just to hand out flavor
    public class FlavorTinkerData : TinkerData
    {
        public Flavor flavor;
    }

    // ( Status -> tinker (-> data) -> flavor )

    public interface IStatus
    {
        // sets up the tinker + flavor on the tinker data
        public void Apply(Entity entity, Flavor f);
        // ticks the tinker and removes status if necessary
        public void Tick(Entity entity);
        public bool IsApplied(Entity entity);
    }

    public class Status<T> : IStatus
        where T : FlavorTinkerData, new()
    {
        Tinker<T> m_tinker;
        public Status(Tinker<T> tinker)
        {
            m_tinker = tinker;
        }
        public void Apply(Entity entity, Flavor f)
        {
            entity.TinkAndSave(m_tinker);
            var store = m_tinker.GetStore(entity.id);
            store.flavor = f;
        }

        public void Tick(Entity entity)
        {
            var store = m_tinker.GetStore(entity.id);
            if (store != null && --store.flavor.amount <= 0)
            {
                entity.Untink(m_tinker);
            }
        }

        public bool IsApplied(Entity entity)
        {
            return m_tinker.IsApplied(entity.id);
        }
    }

    // this is questionable
    public class StatusContext
    {
        IStatus status;
        Entity entity;

        public StatusContext(IStatus status, Entity entity)
        {
            this.status = status;
            this.entity = entity;
        }

        public void Apply(Flavor flavor) => status.Apply(entity, flavor);
        public void Tick() => status.Tick(entity);
    }
}