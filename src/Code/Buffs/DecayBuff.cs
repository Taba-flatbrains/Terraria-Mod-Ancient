using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Ancient.src.Code.Buffs
{
    internal class DecayBuff : ModBuff // does not work yet, dont use
    {
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<DecayBuffPlayer>().active = true;
        }
    }

    internal class DecayBuffPlayer : ModPlayer 
    {
        public int Intensity = 0;
        public bool active = false;

        public override void ResetEffects()
        {
            if (!active)
            {
                Intensity = 0;
            }
            active = false;
        }

        public override void ModifyMaxStats(out StatModifier health, out StatModifier mana)
        {
            base.ModifyMaxStats(out health, out mana); // wichtige line nicht removen
            health -= Math.Min(Intensity, Player.statLifeMax2 - 100); // funktioniert nicht
        }
    }
}
