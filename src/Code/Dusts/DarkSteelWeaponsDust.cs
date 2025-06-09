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
    internal class DarkSteelWeaponsDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.velocity = new Vector2((new Random().NextSingle() - 0.5f) * 0.5f, (new Random().NextSingle() - 0.5f) * 0.5f);
            dust.frame = new Rectangle(0, Main.rand.Next(3) * 8, 8, 8);
        }

        public override bool Update(Dust dust)
        {
            // Move the dust based on its velocity and reduce its size to then remove it, as the 'return false;' at the end will prevent vanilla logic.
            //dust.rotation += 1;


            Lighting.AddLight(dust.position, new Vector3(0.5f, 0, 0));
            dust.scale -= 0.010f;

            if (dust.scale < 0.75f)
                dust.active = false;

            return false;
        }
    }

    internal class DarkSteelWeaponsDust2 : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.rotation = new Random().NextSingle() * MathHelper.TwoPi;
            dust.frame = new Rectangle(0, Main.rand.Next(3) * 8, 8, 8);
        }

        public override bool Update(Dust dust)
        {
            // Move the dust based on its velocity and reduce its size to then remove it, as the 'return false;' at the end will prevent vanilla logic.
            //dust.rotation += 1;


            dust.scale -= 0.040f;

            if (dust.scale < 0.75f)
                dust.active = false;

            return false;
        }
    }
}
