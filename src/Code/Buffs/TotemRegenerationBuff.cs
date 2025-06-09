using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Ancient.src.Code.Buffs
{
    internal class TotemRegenerationBuff : ModBuff
    {
    }

    internal class TotemRegenerationBuffPlayer : ModPlayer
    {
        public override void UpdateLifeRegen()
        {
            if (Player.HasBuff<TotemRegenerationBuff>())
            {
                Player.lifeRegen += 5;
            }
        }
    }
}
