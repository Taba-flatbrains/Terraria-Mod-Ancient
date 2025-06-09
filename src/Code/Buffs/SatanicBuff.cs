using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Ancient.src.Code.Buffs
{
    internal class SatanicBuff : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<SatanicBuffPlayer>().active = true;
        }
    }

    public class SatanicBuffPlayer : ModPlayer
    {
        public bool active = false;

        public override void ResetEffects()
        {
            active = false;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (active)
            {
                modifiers.SetCrit();
            }
        }
    }
}
