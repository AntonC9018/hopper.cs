using System.Collections.Generic;
using System.Linq;
using Hopper.Core.ActingNS;
using Hopper.Core.Components;
using Hopper.Core.WorldNS;
using Hopper.Shared.Attributes;
using Hopper.Utils.Vector;

namespace Hopper.Core.Targeting
{
    public partial class UnbufferedTargetProvider : IComponent
    {
        [Inject] public IGeneralizedPattern _pattern;
        [Inject] public Layers _targetedLayer;
        [Inject] public Faction _targetedFaction; 

        public IEnumerable<TargetContext> GetTargets(IntVector2 position, IntVector2 direction)
        {
            foreach (var pd in _pattern.GetPositionsAndDirections(position, direction))
            {
                var matches = World.Global.Grid.GetAllFromLayer(pd.position, pd.direction, _targetedLayer)
                    .Where(t1 => !t1.entity.TryCheckFaction(_targetedFaction, out bool result) || result);

                if (matches.Any())
                {
                    yield return new TargetContext(pd.position, pd.direction, matches.First());
                }
            }
        }

        public IEnumerable<TargetContext> GetTargetsDeep(IntVector2 position, IntVector2 direction)
        {
            foreach (var t in _pattern.GetPositionsAndDirections(position, direction))
            {
                var matches = World.Global.Grid.GetAllFromLayer(t.position, t.direction, _targetedLayer)
                    .Where(t1 => t1.entity.TryCheckFaction(_targetedFaction));

                foreach (var transform in matches)
                {
                    yield return new TargetContext(t.position, t.direction, transform);
                }
            }
        }

        public bool WhetherMayAffect(Layers targetLayer, Faction targetFaction)
        {
            return _targetedFaction.HasEitherFlag(targetFaction) && _targetedLayer.HasEitherFlag(targetLayer);
        }

        /// <summary>
        /// This function returns positions that could target the entity 
        /// from the given layer and of the given faction if the GetTargets()
        /// function were to be evaluated with the given position and direction.
        /// If the target provider may never target the given layer / faction,
        /// this returns an empty enumerable.
        /// </summary>
        public IEnumerable<IntVector2> PredictPositions(
            IntVector2 position, IntVector2 direction, PredictionTargetInfo info)
        {
            return PredictPositionsAndDirections(position, direction, info).Select(pd => pd.position);
        }

        /// <summary>
        /// This function returns positions and directions that could target the entity 
        /// from the given layer and of the given faction if the GetTargets()
        /// function were to be evaluated with the given position and direction.
        /// If the target provider may never target the given layer / faction,
        /// this returns an empty enumerable.
        /// </summary>
        public IEnumerable<PositionAndDirection> PredictPositionsAndDirections(
            IntVector2 position, IntVector2 direction, PredictionTargetInfo info)
        {
            if (WhetherMayAffect(info.layer, info.faction))
            {
                return _pattern.GetPositionsAndDirections(position, direction);
            }
            return Enumerable.Empty<PositionAndDirection>();
        }

        /// <summary>
        /// This function returns positions that could target the entity 
        /// if the GetTargets() function were to be evaluated with the given position and direction.
        /// If the target provider may never target the entities' layer / faction,
        /// this returns an empty enumerable.
        /// </summary>
        // public IEnumerable<IntVector2> PredictPositionsFor(
        //     IntVector2 position, IntVector2 direction, Entity potentialTarget)
        // {
        //     return PredictPositionsAndDirectionsFor(position, direction, potentialTarget).Select(pd => pd.position);
        // }

        /// <summary>
        /// This function returns positions and directions that could target the entity 
        /// if the GetTargets() function were to be evaluated with the given position and direction.
        /// If the target provider may never target the entities' layer / faction,
        /// this returns an empty enumerable.
        /// </summary>
        // public IEnumerable<PositionAndDirection> PredictPositionsAndDirectionsFor(
        //     IntVector2 position, IntVector2 direction, Entity potentialTarget)
        // {
            
        //     var layer = potentialTarget.GetTransform().layer;
        //     var faction = potentialTarget.TryGetFactionComponent(out var f) ? f.faction : Faction.Any;
        //     return PredictPositionsAndDirectionsFor(position, direction, layer, faction);
        // }

        /// <summary>
        /// This function returns the positions that would be targeted by the given
        /// entity if the GetTargets() function were to be evaluated at their position.
        /// </summary>
        public IEnumerable<IntVector2> PredictPositionsBy(
            Entity actor, IntVector2 direction, PredictionTargetInfo info)
        {
            return PredictPositions(actor.GetTransform().position, direction, info);
        }

        public IEnumerable<PositionAndDirection> PredictPositionsAndDirectionsBy(
            Entity actor, IntVector2 direction, PredictionTargetInfo info)
        {
            return PredictPositionsAndDirections(actor.GetTransform().position, direction, info);
        }
    }
}