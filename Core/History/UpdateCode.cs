using System.Collections.Generic;
using Core.Utils;

namespace Core.History
{
    public class UpdateCode : ScalableEnum
    {
        private static List<UpdateCode> updateCodes = new List<UpdateCode>();

        // this one is always added to the history once it is cleared.
        public static readonly UpdateCode control = new UpdateCode("control");

        public static readonly UpdateCode attacking_do = new UpdateCode("attacking_do");
        public static readonly UpdateCode attacked_do = new UpdateCode("attacked_do");
        public static readonly UpdateCode displaced_do = new UpdateCode("displaced_do");
        public static readonly UpdateCode move_do = new UpdateCode("move_do");
        public static readonly UpdateCode pushed_do = new UpdateCode("pushed_do");
        public static readonly UpdateCode statused_do = new UpdateCode("statused_do");
        public static readonly UpdateCode hurt = new UpdateCode("hurt");
        public static readonly UpdateCode dead = new UpdateCode("dead");

        public UpdateCode(string name) : base(name, updateCodes.Count)
        {
            updateCodes.Add(this);
        }

        public static implicit operator UpdateCode(int value)
        {
            return updateCodes[value];
        }
    }
}