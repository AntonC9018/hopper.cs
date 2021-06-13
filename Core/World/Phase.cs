using Hopper.Core.ActingNS;

namespace Hopper.Core.WorldNS
{
    public enum Phase
    {   
        // Before any of the orders, start is the phase
        // Start = 0,

        // These few are the same as Order
        // These are for acting
        Player_Act = Order.Player, Entity_Act, Trap_Act, Projectile_Act,

        // Then there is ticking
        // Set up so that incrementing gets you to the next one
        Ticking,

        // Action calculation
        Calculate_Actions,

        // Filtering the dead entities
        FilterDead,

        // The loop has finished
        Done
    }

    public static class PhaseExtensions
    {
        public static bool IsOrder(this Phase phase)
        {
            int iphase = (int) phase;
            if (iphase >= (int) Order.Player && iphase <= (int) Order.Projectile)
            {
                return true;
            }
            return false;
        }
    }
}