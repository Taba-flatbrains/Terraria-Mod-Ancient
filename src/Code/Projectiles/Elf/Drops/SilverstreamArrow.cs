using Ancient.src.Code.Dusts.ElfInvasion;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace Ancient.src.Code.Projectiles.Elf.Drops
{
    internal class SilverstreamArrow : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ItemID.WoodenArrow);

            Projectile.width = 14; // The width of projectile hitbox
            Projectile.height = 32; // The height of projectile hitbox
            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
            Projectile.DamageType = DamageClass.Ranged; // Is the projectile shoot by a ranged weapon?
            Projectile.penetrate = 2; // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 1200; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = true; // Can the projectile collide with tiles?

            Projectile.aiStyle = ProjAIStyleID.Arrow;
        }

        private int ticks = 0;
        public override void AI()
        {
            ticks++;
            if (new Random().Next(5) == 0)
            {
                Dust.NewDust(Projectile.position, 14, 14, ModContent.DustType<SilverstreamDust>());
            }
            if (Projectile.ai[1] == 1f)
            {
                Lighting.AddLight(Projectile.Center, new Vector3(0, 0.2f, 0.5f));
            }

            if (Main.myPlayer == Projectile.owner)
            {
                if (ticks % 20 == 0)
                {
                    if (new Random().Next(3) == 0 && Projectile.ai[1] == 0f)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            Vector2 NewVelocity = Projectile.velocity.RotatedByRandom(0.3f);
                            Dust.NewDust(Projectile.position + NewVelocity, 14, 14, DustID.IceTorch);
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.position, NewVelocity, Type, Projectile.damage, Projectile.knockBack, Projectile.owner, 0, 1);
                        }
                        Projectile.active = false;
                    }
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (new Random().Next(3) == 0 && Projectile.ai[1] == 0f && Main.myPlayer == Projectile.owner)
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector2 NewVelocity = Projectile.velocity.RotatedByRandom(0.3f);
                    Dust.NewDust(Projectile.position + NewVelocity, 14, 14, DustID.IceTorch);
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.position, NewVelocity, Type, Projectile.damage, Projectile.knockBack, Projectile.owner, 0, 1);
                }
                Projectile.active = false;
            }
        }
    }
}
