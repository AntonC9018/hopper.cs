using Core;

namespace Core.Weapon
{

    public static class Handlers
    {
        public static void NextToAny(Weapon.Event weaponEvent)
        {
            var first = weaponEvent.targets[0];
            if (first.index == 0 && first.attackableness != AtkCondition.NEVER)
                return;
            if (weaponEvent.targets.Any(t => t.attackableness == AtkCondition.IF_NEXT_TO
        }
        // utils.nextToAny = function(event)

        //     -- otherwise, check if not everything is attackable only when we're close
        //     local all = true
        //     for i = 1, #event.targets do
        //         if event.targets[i].attackableness ~= Attackableness.IF_NEXT_TO then
        //             all = false
        //             break
        //         end
        //     end

        //     -- if all are, return nothing as the targets
        //     if all then
        //         event.propagate = false
        //         event.targets = {}
        //     end

        // end

    }
}