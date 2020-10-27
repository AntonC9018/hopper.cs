using System.Collections.Generic;
using Core.Utils.Vector;

namespace Core.Targeting
{
    public class Piece
    {
        public IntVector2 pos;
        public IntVector2 dir;
        // null is no checking required
        // empty list to check all previous indeces
        // list of indices to check the specified indeces
        public List<int> reach;
        public int index;

        public Piece Rotate(double angle)
        {
            return new Piece
            {
                pos = pos.RotateAndRound(angle),
                dir = dir.RotateAndRound(angle),
                reach = reach
            };
        }
    }
}