using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Ancient.src.Code.Buffs
{
    internal class MoltenBuff : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<MoltenBuffPlayer>().active = true;
        }
    }

    internal class MoltenBuffPlayer : ModPlayer
    {
        public int Intensity = 0;
        public bool active = false;
        private int ticks = 0;

        public override void ResetEffects()
        {
            if (!active)
            {
                Intensity = 0;
                ticks = 0;
            }
            active = false;
        }

        public override void PostUpdateBuffs()
        {
            if (!active) { return; }
            if (ticks % 20 == 0)
            {
                Intensity += 1;
            }
            ticks++;
            Player.statDefense -= Intensity;
        }
    }
}
