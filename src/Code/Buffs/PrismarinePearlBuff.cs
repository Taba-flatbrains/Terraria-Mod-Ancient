using Ancient.src.Code.Dusts;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Ancient.src.Code.Buffs
{
    internal class PrismarinePearlBuff : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<PrismarinePearlAnimationPlayer>().active = true;
        }
    }

    public class PrismarinePearlAnimationPlayer : ModPlayer
    {
        public bool active = false;

        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (active)
            {
                Lighting.AddLight(Player.Center, new Vector3(0, 0.1f, 1));


                if (new Random().Next(3)==0) { return; }
                Vector2 offset = new Vector2(16*5, 0).RotatedByRandom(Math.PI*2);
                Dust.NewDust(Player.Center+offset, 0, 0, ModContent.DustType<PrismarinePearlChargingDust>(), SpeedX: -offset.X / 13, SpeedY: -offset.Y/13);
            }
        }

        public override void ResetEffects()
        {
            active = false;
        }
    }
}
