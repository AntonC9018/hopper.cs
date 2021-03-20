using System;
using System.Collections.Generic;

namespace Test.Stats
{

    public class Attack : IStat
    {
        public static readonly Index<Attack> Index = new Index<Attack>();

        public int damage = 1;
        public int pierce = 1;
        public int power = 1;
        
        public void AddToSelf(Attack other)
        {
            damage += other.damage;
            pierce += other.pierce;
            power += other.power;
        }

        public void SubFromSelf(Attack other)
        {
            damage -= other.damage;
            pierce -= other.pierce;
            power -= other.power;
        }
    }

}