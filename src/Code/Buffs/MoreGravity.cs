using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Ancient.src.Code.Buffs
{

    public class MoreGravityBuff : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<MoreGravityBuffPlayer>().active = true;
        }
    }

    public class MoreGravityBuffPlayer : ModPlayer
    {
        public bool active = false;
        public override void ResetEffects()
        {
            active = false;
        }
        public override void PreUpdateMovement()
        {
            if (active)
            {
                Player.velocity.Y += 0.4f;
            }
        }
    }
}
