using System.Collections.Generic;
using Hopper.Utils.Vector;

namespace Hopper.Core.Targeting
{
    public class AttackTargetingContext
    {
        public List<AttackTargetContext> targetContexts;
        public PieceAttackPattern pattern;
        public Entity attacker;
        public IntVector2 attackerPosition;
        public IntVector2 attackDirection;
        public Layer targetedLayer;
        public Layer blockLayer;

        public AttackTargetingContext(List<AttackTargetContext> targetContexts, PieceAttackPattern pattern, Entity attacker, IntVector2 attackerPosition, IntVector2 attackDirection, Layer targetedLayer, Layer blockLayer)
        {
            this.targetContexts = targetContexts;
            this.pattern = pattern;
            this.attacker = attacker;
            this.attackerPosition = attackerPosition;
            this.attackDirection = attackDirection;
            this.targetedLayer = targetedLayer;
        }
    }

    public class TargetContext
    {
        public IntVector2 position;
        public IntVector2 direction;
        public Transform transform;

        public TargetContext(IntVector2 position, IntVector2 direction)
        {
            this.position = position;
            this.direction = direction;
            this.transform = null;
        }
    }

    public class AttackTargetContext : TargetContext
    {
        public Attackness attackness;
        public int pieceIndex;

        public AttackTargetContext(IntVector2 position, IntVector2 direction, int pieceIndex = 0) : base(position, direction)
        {
            this.attackness = Attackness.NEVER;
            this.pieceIndex = pieceIndex;
        }
    }
}