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
            this.blockLayer = blockLayer;
        }
    }

    public struct PositionAndDirection 
    { 
        public IntVector2 position; 
        public IntVector2 direction;

        public PositionAndDirection(IntVector2 position, IntVector2 direction)
        {
            this.position = position;
            this.direction = direction;
        }
    }

    public struct TargetContext
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

        public TargetContext(Transform transform)
        {
            this.transform = transform;
            this.position  = transform.position;
            this.direction = transform.orientation;
        }

        public TargetContext(IntVector2 position, IntVector2 direction, Transform transform)
        {
            this.position = position;
            this.direction = direction;
            this.transform = transform;
        }
    }

    public class AttackTargetContext
    {
        public TargetContext normal;
        public Attackness attackness;
        public int pieceIndex;

        public IntVector2 position { get => normal.position; set => normal.position = value; }
        public IntVector2 direction { get => normal.direction; set => normal.direction = value; }
        public Transform transform { get => normal.transform; set => normal.transform = value; }

        public AttackTargetContext(IntVector2 position, IntVector2 direction, int pieceIndex = 0)
        {
            this.attackness = 0;
            this.pieceIndex = pieceIndex;
            this.normal = new TargetContext(position, direction);
        }

        public AttackTargetContext(Transform transform)
        {
            this.attackness = 0;
            this.pieceIndex = 0;
            this.normal = new TargetContext(transform);
        }
    }
}