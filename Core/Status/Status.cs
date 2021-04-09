using Hopper.Core.Registries;
using Hopper.Core.Stats;
using Hopper.Core.Stats.Basic;
using Hopper.Core.Components.Basic;

namespace Hopper.Core
{
    public interface IStatus : IExtendent
    {
        int SourceId { get; }
        void Update(Entity entity);
        bool IsApplied(Entity entity);
        // void Nullify(Entity entity);
        bool TryApplyAuto(Entity applicant, Entity target);
    }

    public static class Status
    {
        public class Source : SourceBase<Source>
        {
            public static readonly DictPatchWrapper<Source> Resistance
                = new DictPatchWrapper<Source>("status/res");
        }
    }

    public class Status<T> : Kind<IStatus>, IStatus where T : StatusData, new()
    {
        private IStatPath<StatusFile> m_statPath;
        public Tinker<T> m_tinker;
        public Status.Source m_source;

        public int SourceId => m_source.Id;

        public Status(IChainDef[] chainDefs, IStatPath<StatusFile> statPath, int defaultResValue)
        {
            m_statPath = statPath;
            m_tinker = new Tinker<T>(chainDefs);
            m_source = new Status.Source { resistance = defaultResValue };
        }

        public override void RegisterSelf(ModRegistry registry)
        {
            m_tinker.RegisterSelf(registry);
            m_source.RegisterSelf(registry); // think about this. 
            m_id = registry.Add<IStatus>(this);
        }

        public void Patch(PatchArea patchArea)
        {
            m_source.Patch(patchArea);
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
            return m_statPath.Path(entity.GetStats());
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