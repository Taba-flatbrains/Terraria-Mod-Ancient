using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Ancient.src.Code.Buffs
{
    internal class SlipperyBuff : ModBuff
    {

    }

    internal class SlipperyPlayer : ModPlayer 
    {

        public override bool FreeDodge(Player.HurtInfo info)
        {
            if (Player.HasBuff<SlipperyBuff>())
            {
                if (new Random().Next(10) == 0)
                {
                    Player.SetImmuneTimeForAllTypes(Player.longInvince ? 120 : 80);
                    return true;
                }
            }
            return false;
        }
    }
}
