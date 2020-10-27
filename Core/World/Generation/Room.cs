using System;
using Core.Utils.Vector;

namespace Core.Generation
{
    public class Room
    {
        public IntVector2 position;
        public IntVector2 dimensions;
        public Node node;
        public Vector2 Center => (Vector2)position + ((Vector2)DimensionsMinusOne) / 2;
        public IntVector2 DimensionsMinusOne => dimensions - new IntVector2(1, 1);

        public RoomStuff roomStuff;

        public Room(IntVector2 dimensions, Node node)
        {
            this.dimensions = dimensions;
            this.node = node;
        }

        public Vector2 GetBorderCenter(IntVector2 direction)
        {
            return Center + GetCenterOffsetVector(direction);
        }

        public IntVector2 GetDimensionVector(IntVector2 direction)
        {
            return direction.HagamardProduct(dimensions);
        }

        public int GetDimension(IntVector2 direction)
        {
            return Math.Abs(GetDimensionVector(direction).ComponentSum());
        }

        public IntVector2 GetDimensionMinusOneVector(IntVector2 direction)
        {
            return direction.HagamardProduct(DimensionsMinusOne);
        }

        public int GetDimensionMinusOne(IntVector2 direction)
        {
            return Math.Abs(GetDimensionMinusOneVector(direction).ComponentSum());
        }

        public Vector2 GetCenterOffsetVector(IntVector2 direction)
        {
            return ((Vector2)GetDimensionMinusOneVector(direction)) / 2;
        }

        public void SetPositionFromCenter(Vector2 center)
        {
            this.position = (IntVector2)(center - ((Vector2)DimensionsMinusOne) / 2);
        }
    }
}