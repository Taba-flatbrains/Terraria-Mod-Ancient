using Terraria;
using Terraria.ModLoader;

namespace Ancient.src.Code.Dusts
{
    internal class PrismarinePearlChargingDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.alpha = 100;
            dust.scale = 0.9f;
            dust.frame = new Microsoft.Xna.Framework.Rectangle(0, 0, 24, 24);
            dust.rotation = dust.velocity.ToRotation();
        }

        public override bool Update(Dust dust)
        {

            dust.scale -= 0.007f;

            if (dust.scale < 0.5f)
                dust.active = false;


            dust.rotation = dust.velocity.ToRotation();

            Lighting.AddLight(dust.position, new Microsoft.Xna.Framework.Vector3(0.4f, 0.6f, 0.8f));

            return true;
        }
    }
}
