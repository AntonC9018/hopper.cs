namespace Hopper.Core.Targeting
{
    public enum Attackness
    {
        CAN_BE_ATTACKED = 0b_0000_0001,
        BY_DEFAULT = 0b_0000_1000,
        IS_BLOCK = 0b_0000_0010,
        IF_NEXT_TO = 0b_0000_0100,

        ALWAYS = CAN_BE_ATTACKED | BY_DEFAULT | IS_BLOCK, // can be attacked by default
        NEVER = IS_BLOCK,
        MAYBE = CAN_BE_ATTACKED | IS_BLOCK, // can be attacked, not by default
        SKIP = CAN_BE_ATTACKED,             // can be attacked, no block
        CAN_BE_ATTACKED_IF_NEXT_TO = CAN_BE_ATTACKED | BY_DEFAULT | IS_BLOCK | IF_NEXT_TO
    }

    public static class AttacknessExtensions
    {
        public static bool Is(this Attackness attackness, Attackness checkAgainst)
        {
            return (attackness & checkAgainst) != 0;
        }

        public static bool IsNot(this Attackness attackness, Attackness checkAgainst)
        {
            return (attackness & checkAgainst) == 0;
        }
    }
}