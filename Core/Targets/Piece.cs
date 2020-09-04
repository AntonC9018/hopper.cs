using System.Collections.Generic;
using Vector;

namespace Core.Weapon
{
    public class Piece
    {
        public IntVector2 pos;
        public IntVector2 dir;
        // null is no checking required
        // empty list to check all previous indeces
        // list of indices to check the specified indeces
        public List<int> reach;
        public Piece Rotate(double angle)
        {
            return new Piece
            {
                pos = pos.Rotate(angle),
                dir = dir.Rotate(angle),
                reach = reach
            };
        }
    }
}