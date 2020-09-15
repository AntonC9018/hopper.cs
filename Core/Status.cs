using System;
using Core.Behaviors;

namespace Core
{
    // Statused -> status.Apply(entity, flavor)
    // -> entity.TinkAndSave(tinker)
    // +  tinkerData.flavor = flavor
    // now to see if a status is active one needs to
    // 1. get the tinker's id
    public class StatusFile : StatFile
    {
        public int amount;
        public int power;
    }

    public class Flavor
    {
        public int amount;
    }

    public interface IHaveFlavor
    {
        Flavor Flavor { get; set; }
    }

    public interface IHaveSpice
    {
        ITinker Spice { get; }
    }

    // such tinker data is used just to hand out flavor
    public class FlavorTinkerData<T> : TinkerData, IHaveFlavor
        where T : Flavor
    {
        public T flavor;
        public Flavor Flavor
        {
            get => flavor;
            set => flavor = (T)value;
        }
    }

    // ( Status -> tinker (-> data) -> flavor )
    public interface IStatus
    {
        // sets up the tinker + flavor on the tinker data
        void Apply(Entity entity, Flavor f);
        // ticks the tinker and removes status if necessary
        void Tick(Entity entity);
        bool IsApplied(Entity entity);
    }

    public class Status<T> : IStatus
        where T : TinkerData, IHaveFlavor, new()
    {
        private Tinker<T> m_tinker;
        public Status(Tinker<T> tinker)
        {
            m_tinker = tinker;
        }
        public void Apply(Entity entity, Flavor f)
        {
            System.Console.WriteLine("Status applied");
            entity.TinkAndSave(m_tinker);
            var store = m_tinker.GetStore(entity);
            store.Flavor = f;

            // this tinker is not being saved on the entity
            // since it is exclusive to a status effect
            // It probably doesn't even need the personal store, but then how do you manage
            // the untinking
            if (f is IHaveSpice)
            {
                var tinker = ((IHaveSpice)f).Spice;
                entity.TinkAndSave(tinker);
            }
        }

        public void Tick(Entity entity)
        {
            if (!IsApplied(entity))
                return;
            var store = m_tinker.GetStore(entity);
            var f = store.Flavor;

            if (store != null && --f.amount <= 0)
            {
                entity.Untink(m_tinker);

                if (f is IHaveSpice)
                {
                    var tinker = ((IHaveSpice)f).Spice;
                    entity.Untink(tinker);
                }
            }
        }

        public bool IsApplied(Entity entity)
        {
            return entity.IsTinked(m_tinker);
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