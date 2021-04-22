using Hopper.Utils.Chains;
using Hopper.Core.Stat.Basic;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Hopper.Core.Stat;
using Hopper.Shared.Attributes;
using System;

namespace Hopper.Core.Components.Basic
{
    [ActivationAlias("ApplyStatus")]    
    [Chains("Resist")]
    public partial class Statused : IBehavior
    {
        public class Context : ContextBase
        {
            public Entity actor;
            public StatusParam[] statusParams;
            [Omit] public int resistance;
        }

        public HashSet<IStatus> _appliedStatuses = new HashSet<IStatus>();

        // Tick.Chain.ChainPath(m_entity.Behaviors).AddHandler(
        //     e => UpdateStatuses()
        // );

        // Returns a list of the statuses that were applied
        // public IEnumerable<IStatus> Activate(Params pars)
        public bool Activate(Entity actor, StatusParam[] param)
        {
            var ctx = new Context
            {
                actor = actor,
                statusParams = param
            };
            TraverseResist(ctx);
            AddStatuses(ctx.statusParams);
            // foreach (var statusParam in ev.statusParams)
            // {
            //     yield return statusParam.status;
            // }
            return ctx.statusParams.Length > 0;
        }

        [Export(Chain = "Ticking.Do")] private void UpdateStatuses(Entity actor)
        {
            foreach (var status in _appliedStatuses.ToList())
            {
                status.Update(actor);
                if (!status.IsApplied(actor))
                {
                    _appliedStatuses.Remove(status);
                }
            }
        }

        private void AddStatuses(StatusParam[] statusParams)
        {
            foreach (var par in statusParams)
            {
                _appliedStatuses.Add(par.status);
            }
        }

        [Export] public static void SetResistance(Context ctx)
        {
            ctx.resistance = ctx.actor.GetStats().GetLazy(Status.Source.Resistance.Path);
        }

        [Export] public static void ResistSomeStatuses(Context ctx)
        {
            ctx.statusParams = ctx.statusParams
                .Where(p => ctx.resistance[p.status.SourceId] <= p.statusStat.power)
                .Where(p => p.statusStat.amount > 0)
                .ToArray();
        }

        public void DefaultPreset()
        {
            _ResistChain.Add(SetResistanceHandler, ResistSomeStatusesHandler);
        }
    }
}