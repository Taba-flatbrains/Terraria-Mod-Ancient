using Ancient.src.Code.Dusts;
using Ancient.src.Code.Items.Accessoires;
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

namespace Ancient.src.Code.Projectiles.Desolator
{
    internal class DesolatorCircularSwingProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 140; // The width of projectile hitbox
            Projectile.height = 350; // The height of projectile hitbox
            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
            Projectile.DamageType = DamageClass.Melee; // Is the projectile shoot by a ranged weapon?
            Projectile.penetrate = -1; // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 40; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
            Projectile.alpha = 80;
        }

        public override void AI()
        {
            for (int i = 0; i < 20; i++)
            {
                float rotation = MathF.PI / 10 * i;
                int size = (int)(Projectile.height / 4 * Projectile.scale);
                Lighting.AddLight(Projectile.Center + (new Vector2(MathF.Sin(rotation), MathF.Cos(rotation)) * size / 2), new Vector3(1f, 0, 0));
                Lighting.AddLight(Projectile.Center + (new Vector2(MathF.Sin(rotation), MathF.Cos(rotation)) * size), new Vector3(1f, 0, 0));
                if (new Random().Next(20)==0)
                {
                    Dust.NewDust(Projectile.Center + (new Vector2(MathF.Cos(rotation), MathF.Sin(rotation)) * new Random().Next(3, (int)MathF.Round(size) + 3)), Projectile.width, 0, ModContent.DustType<DarkSteelWeaponsDust>());
                }
            }
            Projectile.rotation += MathHelper.Pi / 14;
            Projectile.scale *= 0.99f;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.CritDamage += 0.2f; // + 20% crit damage
        }
    }
}
