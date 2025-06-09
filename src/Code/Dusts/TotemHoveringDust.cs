using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Ancient.src.Code.Dusts
{
    internal class TotemHoveringDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.frame = new Rectangle(0, Main.rand.Next(3) * 8, 8, 8);
            dust.velocity = dust.velocity.RotatedByRandom(0.4);
            dust.velocity *= 0.8f;
            dust.scale = 0.9f;
        }

        public override bool Update(Dust dust)
        {
            Lighting.AddLight(dust.position, dust.color.ToVector3());
            dust.position += dust.velocity;
            dust.scale -= 0.015f;
            dust.velocity *= 0.96f;
            dust.rotation += 0.05f;

            if (dust.scale < 0.7f)
                dust.active = false;

            return false;
        }
    }
}
