using Hopper.Core.Behaviors;
using Hopper.Core.Stats;
using Hopper.Core.Chains;
using Hopper.Core.Stats.Basic;

namespace Hopper.Core
{
    public interface IStatus : IKind
    {
        void Update(Entity entity);
        bool IsApplied(Entity entity);
        // void Nullify(Entity entity);
        bool TryApplyAuto(Entity applicant, Entity target);
    }

    public static class Status
    {
        public static class Resistance
        {
            public static StatPath<ArrayFile> Path =
                new StatPath<ArrayFile>
                (
                    "status/res",
                    ArrayFilePath<IStatus>.GetDefaultFile
                );
        }
    }

    public class Status<T> : IStatus where T : StatusData, new()
    {
        private IStatPath<StatusFile> m_statPath;
        private int m_resDefaultValue;
        private Registry m_registry;
        private int m_id;
        protected Tinker<T> m_tinker;

        public Tinker<T> Tinker => m_tinker;
        public int Id => m_id;
        public SourceBase<IStatus> m_source;

        public Status(IChainDef[] chainDefs, IStatPath<StatusFile> statPath, int defaultResValue)
        {
            m_statPath = statPath;
            m_tinker = new Tinker<T>(chainDefs);
            m_source = new SourceBase<IStatus> { resistance = defaultResValue };
        }

        public void RegisterSelf(Registry registry)
        {
            m_tinker.RegisterSelf(registry);
            m_id = registry.GetKindRegistry<IStatus>().Add(this);
            m_source.InitFor(registry);
        }

        public virtual void Update(Entity entity)
        {
            if (entity.Tinkers.IsTinked(m_tinker))
            {
                var data = m_tinker.GetStore(entity);
                UpdateAmount(data);

                if (data.amount <= 0)
                {
                    Remove(entity);
                }
            }
        }

        protected virtual void UpdateAmount(T store)
        {
            store.amount--;
        }

        public virtual bool IsApplied(Entity entity)
        {
            return m_tinker.IsTinked(entity) && m_tinker.GetStore(entity).amount > 0;
        }

        public bool TryApplyAuto(Entity applicant, Entity target)
            => TryApplyWithInitialData(applicant, target, new T());

        public bool TryApplyWithInitialData(Entity applicant, Entity target, T statusData)
            => TryApply(target, statusData, GetStat(applicant));

        // A convenience method for calling the Statused decorator
        public bool TryApply(Entity target, T statusData, StatusFile stat)
        {
            if (target.Behaviors.Has<Statused>() == false)
            {
                return false;
            }
            if (m_tinker.IsTinked(target))
            {
                Reapply(m_tinker.GetStore(target), statusData);
                return true;
            }

            statusData.amount = stat.amount;
            var pars = new Statused.Params(new StatusParam(this, stat));
            bool success = target.Behaviors.Get<Statused>().Activate(pars);

            if (success)
            {
                Apply(target, statusData);
            }

            return success;
        }

        // By default, just update the amount
        protected virtual void Reapply(T existingData, T newData)
        {
            existingData.amount = newData.amount;
        }

        public StatusFile GetStat(Entity entity)
        {
            return m_statPath.Path(entity.Stats);
        }

        protected virtual void Apply(Entity target, T statusData)
        {
            m_tinker.Tink(target, statusData);
        }

        public virtual void Remove(Entity entity)
        {
            m_tinker.Untink(entity);
        }
    }
}