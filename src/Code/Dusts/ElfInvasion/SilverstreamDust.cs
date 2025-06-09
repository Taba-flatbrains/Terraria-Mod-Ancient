using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Ancient.src.Code.Dusts.ElfInvasion
{
    internal class SilverstreamDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.frame = new Rectangle(0, Main.rand.Next(3) * 10, 10, 10);
        }

        public override bool Update(Dust dust)
        {
            Lighting.AddLight(dust.position, new Vector3(0.3f, 0.3f, 0.3f));
            dust.position += dust.velocity;
            dust.scale -= 0.015f;

            if (dust.scale < 0.75f)
                dust.active = false;

            return false;
        }
    }
}
