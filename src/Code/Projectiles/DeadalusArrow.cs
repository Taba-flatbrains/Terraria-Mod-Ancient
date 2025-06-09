using Ancient.src.Code.Dusts;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Ancient.src.Code.Projectiles
{
    internal class DeadalusArrow : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ItemID.WoodenArrow);

            Projectile.width = 36; // The width of projectile hitbox
            Projectile.height = 18; // The height of projectile hitbox
            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
            Projectile.DamageType = DamageClass.Ranged; // Is the projectile shoot by a ranged weapon?
            Projectile.penetrate = 5; // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 1200; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = true; // Can the projectile collide with tiles?

            Projectile.aiStyle = ProjAIStyleID.Arrow;
        }


        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.CritDamage += 0.2f; // + 20% crit damage
        }

        public override void AI()
        {
            base.AI();
            Projectile.rotation -= MathHelper.PiOver2;
            Projectile.velocity *= 1.03f;

            if (new Random().Next(5) == 0)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<DarkSteelWeaponsDust>());
            }

            for (int i = 0; i < 5; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<DarkSteelWeaponsDust2>());
            }
            Lighting.AddLight(Projectile.position, new Vector3(0.1f, 0, 0));
        }
    }
}
