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
    internal class LivingwoodBarrierDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = false;
            dust.velocity = new Vector2((new Random().NextSingle()-0.5f)*4, (new Random().NextSingle()-0.5f)*4-1f);
            dust.frame = new Rectangle(0, Main.rand.Next(3) * 8, 8, 8);
        }

        public override bool Update(Dust dust)
        {
            // Move the dust based on its velocity and reduce its size to then remove it, as the 'return false;' at the end will prevent vanilla logic.
            //dust.rotation += 1;


            dust.scale -= 0.010f;

            if (dust.scale < 0.75f)
                dust.active = false;

            return true;
        }
    }
}
