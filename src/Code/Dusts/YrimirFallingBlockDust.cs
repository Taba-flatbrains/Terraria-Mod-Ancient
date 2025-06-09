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
    internal class YrimirFallingBlockDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
        }

        public override bool Update(Dust dust)
        {
            Lighting.AddLight(dust.position, new Vector3(0f, 0.1f, 0.4f));
            dust.position.Y += 0.01f;
            dust.scale -= 0.005f;

            if (dust.scale < 0.75f)
                dust.active = false;

            return false;
        }
    }
}
