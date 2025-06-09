using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.ID;

namespace Ancient.src.Code.Projectiles.HeroSword
{
    internal class HeroSwordSwordProjectile : ModProjectile
    {
        private static readonly int duration = 15;
        public override void SetDefaults()
        {
            Projectile.width = 70; // The width of projectile hitbox
            Projectile.height = 70; // The height of projectile hitbox
            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
            Projectile.DamageType = DamageClass.Melee; // Is the projectile shoot by a ranged weapon?
            Projectile.penetrate = -1; // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = duration; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
            Projectile.rotation = -MathHelper.PiOver4;
            Projectile.scale = 0.9f;
        }


        private int ticks = 0;
        public override void AI()
        {
            if (Projectile.velocity.X > 0)
            {
                Projectile.spriteDirection = 1;

                Projectile.rotation += MathF.PI / 3 * 2 / duration;
            } else
            {
                Projectile.spriteDirection = -1;

                Projectile.rotation -= MathF.PI / 3 * 2 / duration;
            }

            if (new Random().Next(3) != 0) { return; }
            float offset = 0;
            int direction = 1;
            if (Projectile.spriteDirection < 0) { offset = MathF.PI; direction = -1; }
            Vector2 TipPosition = Projectile.Center + new Vector2(100, 0).RotatedBy(-direction * (ticks * Math.PI / 25) + offset);
            Dust.NewDust(TipPosition, 3, 3, DustID.Terra);
        }

        private readonly int ShadowBackwardsRotationMultiplier = 1;
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 origin;
            float rotationOffset;
            SpriteEffects effects;
            if (Projectile.spriteDirection > 0)
            {
                origin = new Vector2(0, Projectile.height);
                rotationOffset = MathHelper.ToRadians(45f);
                effects = SpriteEffects.None;
            }
            else
            {
                origin = new Vector2(Projectile.width, Projectile.height);
                rotationOffset = MathHelper.ToRadians(135f);
                effects = SpriteEffects.FlipHorizontally;
            }


            Main.instance.LoadProjectile(Projectile.type);
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            int a = 0;
            for (int k = 0; k < 1; k++)
            {
                
                Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, new Color(new Vector4(lightColor.ToVector3(), (k * 20) + (a * 150))), Projectile.rotation - (k * MathF.PI / 3 / duration * ShadowBackwardsRotationMultiplier * -Projectile.spriteDirection) + rotationOffset, origin, Projectile.scale, effects, 0);
                a = 1;
            }

            return false;
        }
    }
}
