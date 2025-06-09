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
    internal class RockDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = false;
            dust.frame = new Rectangle(0, Main.rand.Next(3) * 12, 12, 12);
            dust.velocity *= 0.9f;
            dust.velocity.Y += 10;
        }

        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;
            dust.scale -= 0.002f;
            dust.rotation += 0.06f * MathF.Sin(dust.dustIndex);
            dust.velocity += new Vector2(0, 0.1f);

            if (dust.scale < 0.8f)
            {
                dust.active = false;
                return false;
            }

            return false;
        }
    }
}
