using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace Ancient.src.Code.Projectiles.HeroSword
{
    internal class HeroSwordSoldier : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 40; // The width of projectile hitbox
            Projectile.height = 54; // The height of projectile hitbox
            Projectile.friendly = false; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
            Projectile.DamageType = DamageClass.Melee; // Is the projectile shoot by a ranged weapon?
            Projectile.penetrate = -1; // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 270; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
            Projectile.alpha = 100;
        }

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 2;
        }

        private int ticks = 0;
        private NPC target;
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, new Vector3(0.05f, 0.7f, 0.2f));
            if (Projectile.velocity.X > 0)
            {
                Projectile.direction = 1;
            }
            else
            {
                Projectile.direction = -1;
            }

            ticks++;
            if (ticks < 150)
            {
                idle();
            }
            else
            {
                if (target == null)
                {
                    findTarget();
                } else if (!target.active)
                {
                    findTarget();
                }
                if (!Projectile.active)
                {
                    return;
                }

                if (ticks < 200)
                {
                    charge();
                }
                else
                {
                    Projectile.frame = 1;
                    Projectile.alpha = 180;
                    chase();
                }
            }
        }

        private void idle()
        {
            Projectile.position += new Vector2(0, MathF.Sin(ticks / 15f)/3);
        }

        private static readonly int MaxVelocity = 10; 
        private void charge()
        {
            Projectile.velocity = target.Center - Projectile.Center;
            if (Projectile.velocity.Length() < 16 * 5)
            {   
                ticks = 200;
            }
            if (Projectile.velocity.Length() > MaxVelocity)
            {
                Projectile.velocity.Normalize();
                Projectile.velocity *= MaxVelocity;
            }
        }

        private void chase()
        {
            if (ticks % 20 == 0)
            {
                Projectile.velocity = target.Center - Projectile.Center;
                if ((target.Center - Projectile.Center).Length() < 0.5f)
                {
                    Projectile.velocity = new Vector2(1, 0);
                }
                else if ((target.Center - Projectile.Center).Length() < 100)
                {
                    Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.PiOver4 / 2);
                }
                Projectile.velocity.Normalize();
                Projectile.velocity *= MaxVelocity;

                if (Main.myPlayer ==Projectile.owner)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + Projectile.velocity + new Vector2(6*Projectile.spriteDirection, -22),
                        Projectile.velocity, ModContent.ProjectileType<HeroSwordSwordProjectile>(), Projectile.damage, Projectile.knockBack, Owner: Main.myPlayer);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // Get the sprite and other drawing properties
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 position = Projectile.Center - Main.screenPosition;
            Rectangle sourceRectangle = new Rectangle(0, texture.Height / Main.projFrames[Type] * Projectile.frame, texture.Width, texture.Height/ Main.projFrames[Type]);
            float rotation = Projectile.rotation;
            Vector2 origin = new Vector2(texture.Width / 2, texture.Height / 2);

            // Mirror the sprite horizontally

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.direction == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }

            // Draw the sprite with the specified sprite effects)
            Main.EntitySpriteDraw(texture,
                position,
                sourceRectangle,
                lightColor,
                rotation,
                origin,
                Projectile.scale,
                spriteEffects,
                0f);

            // Return false to indicate that we've manually drawn the projectile
            return false;
        }

        private void findTarget()
        {
            target = Main.npc.Where((npc) => (npc.active && !npc.friendly && !npc.CountsAsACritter)).MinBy((npc) => Vector2.Distance(Projectile.Center, npc.Center));
            if (target == null)
            {
                Projectile.active = false;
            }
            else if (Vector2.Distance(Projectile.Center, target.Center) > 1500)
            {
                Projectile.active = false;
            }
        }
    }
}
